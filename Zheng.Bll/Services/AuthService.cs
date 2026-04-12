using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Zheng.Bll.Interfaces;
using Zheng.Infra.Data.Models;

namespace Zheng.Bll.Services
{
    public class AuthService : IAuthService
    {
        private readonly LibraryContext _context;
        private readonly IConfiguration _configuration;
        private static readonly PasswordHasher<object> _identityHasher = new();

        public AuthService(LibraryContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public bool VerifyPassword(string hashedPassword, int hashVersion, string providedPassword)
        {
            if (hashVersion == 1)
            {
                // Identity V3 PBKDF2 format — use Identity's PasswordHasher to verify
                var result = _identityHasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);
                return result == PasswordVerificationResult.Success
                    || result == PasswordVerificationResult.SuccessRehashNeeded;
            }

            // V2 = BCrypt
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }

        public async Task<AuthResult> ValidatePasswordAsync(string userName, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.NormalizedUserName == userName.ToUpperInvariant() && u.IsActive);

            if (user == null)
            {
                return new AuthResult { Succeeded = false, Error = "無效的帳號或密碼" };
            }

            // Check email confirmed
            if (!user.EmailConfirmed)
            {
                return new AuthResult { Succeeded = false, Error = "請先完成電子郵件驗證" };
            }

            // Check lockout
            if (user.LockoutEnabled && user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                return new AuthResult { Succeeded = false, Error = "帳號已被鎖定，請稍後再試" };
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return new AuthResult { Succeeded = false, Error = "無效的帳號或密碼" };
            }

            // Verify password
            var isValid = VerifyPassword(user.PasswordHash, user.PasswordHashVersion, password);

            if (!isValid)
            {
                // Increment failed count
                user.AccessFailedCount++;
                if (user.LockoutEnabled && user.AccessFailedCount >= 5)
                {
                    user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(15);
                }
                await _context.SaveChangesAsync();

                return new AuthResult { Succeeded = false, Error = "無效的帳號或密碼" };
            }

            // Password valid — reset failed count
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;

            // Auto-upgrade V1 password to V2 (BCrypt)
            if (user.PasswordHashVersion == 1)
            {
                user.PasswordHash = HashPassword(password);
                user.PasswordHashVersion = 2;
                user.SecurityStamp = Guid.NewGuid().ToString();
            }

            await _context.SaveChangesAsync();

            return new AuthResult { Succeeded = true, User = user };
        }

        public async Task<string> GenerateEmailVerificationTokenAsync(Guid userId)
        {
            var tokenBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(tokenBytes);

            var jwtKey = _configuration.GetSection("JWT")["KEY"];
            using var hmac = new HMACSHA256(System.Text.Encoding.UTF8.GetBytes(jwtKey!));
            var signedBytes = hmac.ComputeHash(tokenBytes);
            var tokenValue = Convert.ToBase64String(tokenBytes) + "." + Convert.ToBase64String(signedBytes);

            var userToken = new UserToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenType = "EmailVerification",
                TokenValue = tokenValue,
                ExpiryDate = DateTime.UtcNow.AddHours(24),
                IsUsed = false,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserTokens.Add(userToken);
            await _context.SaveChangesAsync();

            return tokenValue;
        }

        public async Task<bool> VerifyEmailTokenAsync(Guid userId, string token)
        {
            var userToken = await _context.UserTokens
                .FirstOrDefaultAsync(t =>
                    t.UserId == userId
                    && t.TokenType == "EmailVerification"
                    && t.TokenValue == token
                    && !t.IsUsed
                    && !t.IsRevoked);

            if (userToken == null)
                return false;

            if (userToken.ExpiryDate < DateTime.UtcNow)
                return false;

            userToken.IsUsed = true;

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.EmailConfirmed = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Guid userId)
        {
            var tokenBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(tokenBytes);

            var jwtKey = _configuration.GetSection("JWT")["KEY"];
            using var hmac = new HMACSHA256(System.Text.Encoding.UTF8.GetBytes(jwtKey!));
            var signedBytes = hmac.ComputeHash(tokenBytes);
            var tokenValue = Convert.ToBase64String(tokenBytes) + "." + Convert.ToBase64String(signedBytes);

            var userToken = new UserToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenType = "PasswordReset",
                TokenValue = tokenValue,
                ExpiryDate = DateTime.UtcNow.AddHours(1),
                IsUsed = false,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserTokens.Add(userToken);
            await _context.SaveChangesAsync();

            return tokenValue;
        }

        public async Task<bool> VerifyPasswordResetTokenAsync(Guid userId, string token)
        {
            var userToken = await _context.UserTokens
                .FirstOrDefaultAsync(t =>
                    t.UserId == userId
                    && t.TokenType == "PasswordReset"
                    && t.TokenValue == token
                    && !t.IsUsed
                    && !t.IsRevoked);

            if (userToken == null)
                return false;

            if (userToken.ExpiryDate < DateTime.UtcNow)
                return false;

            userToken.IsUsed = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
