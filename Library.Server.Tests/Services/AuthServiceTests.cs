using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Zheng.Bll.Interfaces;
using Zheng.Bll.Services;
using Zheng.Infra.Data.Models;

namespace Library.Server.Tests.Services
{
    public class AuthServiceTests : IDisposable
    {
        private readonly LibraryContext _context;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LibraryContext(options);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "JWT:KEY", "TestSecretKey1234567890ABCDEFGHIJKLMNOP" }
                })
                .Build();

            _service = new AuthService(_context, config);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private User CreateTestUser(string userName, string password, bool emailConfirmed = true, int hashVersion = 2)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                NormalizedUserName = userName.ToUpperInvariant(),
                Email = $"{userName}@example.com",
                NormalizedEmail = $"{userName.ToUpperInvariant()}@EXAMPLE.COM",
                EmailConfirmed = emailConfirmed,
                PasswordHash = hashVersion == 2 ? _service.HashPassword(password) : password,
                PasswordHashVersion = hashVersion,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };
            return user;
        }

        #region ValidatePasswordAsync

        [Fact]
        public async Task ValidatePassword_CorrectPassword_ReturnsSuccess()
        {
            // Arrange
            var user = CreateTestUser("alice", "Password123!");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.ValidatePasswordAsync("alice", "Password123!");

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.User);
            Assert.Equal(user.Id, result.User.Id);
        }

        [Fact]
        public async Task ValidatePassword_WrongPassword_IncrementsFailCount()
        {
            // Arrange
            var user = CreateTestUser("bob", "Correct123!");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.ValidatePasswordAsync("bob", "WrongPassword!");

            // Assert
            Assert.False(result.Succeeded);
            var updatedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);
            Assert.Equal(1, updatedUser.AccessFailedCount);
        }

        [Fact]
        public async Task ValidatePassword_LockedAccount_ReturnsLocked()
        {
            // Arrange
            var user = CreateTestUser("charlie", "Password123!");
            user.LockoutEnd = DateTimeOffset.UtcNow.AddHours(1);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.ValidatePasswordAsync("charlie", "Password123!");

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("帳號已被鎖定，請稍後再試", result.Error);
        }

        [Fact]
        public async Task ValidatePassword_EmailNotConfirmed_ReturnsError()
        {
            // Arrange
            var user = CreateTestUser("dave", "Password123!", emailConfirmed: false);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.ValidatePasswordAsync("dave", "Password123!");

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("請先完成電子郵件驗證", result.Error);
        }

        [Fact]
        public async Task ValidatePassword_FiveFailures_LocksAccount()
        {
            // Arrange
            var user = CreateTestUser("eve", "Password123!");
            user.AccessFailedCount = 4; // 4 previous failures
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.ValidatePasswordAsync("eve", "WrongPassword!");

            // Assert
            Assert.False(result.Succeeded);
            var updatedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);
            Assert.Equal(5, updatedUser.AccessFailedCount);
            Assert.NotNull(updatedUser.LockoutEnd);
            Assert.True(updatedUser.LockoutEnd > DateTimeOffset.UtcNow);
        }

        [Fact]
        public async Task ValidatePassword_SuccessResetsFailCount()
        {
            // Arrange
            var user = CreateTestUser("frank", "Password123!");
            user.AccessFailedCount = 3;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.ValidatePasswordAsync("frank", "Password123!");

            // Assert
            Assert.True(result.Succeeded);
            var updatedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);
            Assert.Equal(0, updatedUser.AccessFailedCount);
            Assert.Null(updatedUser.LockoutEnd);
        }

        [Fact]
        public async Task ValidatePassword_V1Password_UpgradesToV2()
        {
            // Arrange — use Identity PasswordHasher to create V1 hash
            var identityHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
            var v1Hash = identityHasher.HashPassword(null!, "OldPassword123!");

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "legacy",
                NormalizedUserName = "LEGACY",
                Email = "legacy@example.com",
                NormalizedEmail = "LEGACY@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = v1Hash,
                PasswordHashVersion = 1,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.ValidatePasswordAsync("legacy", "OldPassword123!");

            // Assert
            Assert.True(result.Succeeded);
            var updatedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);
            Assert.Equal(2, updatedUser.PasswordHashVersion);
            Assert.NotEqual(v1Hash, updatedUser.PasswordHash);
            // Verify the new hash is BCrypt format (starts with $2)
            Assert.StartsWith("$2", updatedUser.PasswordHash);
        }

        [Fact]
        public async Task ValidatePassword_NonExistentUser_ReturnsFailure()
        {
            // Act
            var result = await _service.ValidatePasswordAsync("nonexistent", "Password123!");

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("無效的帳號或密碼", result.Error);
        }

        #endregion

        #region HashPassword / VerifyPassword

        [Fact]
        public void HashPassword_ReturnsBCryptHash()
        {
            var hash = _service.HashPassword("TestPassword123!");
            Assert.StartsWith("$2", hash);
        }

        [Fact]
        public void VerifyPassword_V2BCrypt_CorrectPassword_ReturnsTrue()
        {
            var hash = _service.HashPassword("TestPassword123!");
            Assert.True(_service.VerifyPassword(hash, 2, "TestPassword123!"));
        }

        [Fact]
        public void VerifyPassword_V2BCrypt_WrongPassword_ReturnsFalse()
        {
            var hash = _service.HashPassword("TestPassword123!");
            Assert.False(_service.VerifyPassword(hash, 2, "WrongPassword!"));
        }

        [Fact]
        public void VerifyPassword_V1Identity_CorrectPassword_ReturnsTrue()
        {
            var identityHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
            var hash = identityHasher.HashPassword(null!, "TestPassword123!");
            Assert.True(_service.VerifyPassword(hash, 1, "TestPassword123!"));
        }

        #endregion

        #region Email Verification Token

        [Fact]
        public async Task GenerateEmailToken_CreatesTokenInDB()
        {
            // Arrange
            var user = CreateTestUser("tokenuser", "Password123!");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var token = await _service.GenerateEmailVerificationTokenAsync(user.Id);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));
            var dbToken = await _context.UserTokens.FirstOrDefaultAsync(t => t.UserId == user.Id && t.TokenType == "EmailVerification");
            Assert.NotNull(dbToken);
            Assert.False(dbToken.IsUsed);
            Assert.True(dbToken.ExpiryDate > DateTime.UtcNow);
        }

        [Fact]
        public async Task VerifyEmailToken_ValidToken_MarksUsed()
        {
            // Arrange
            var user = CreateTestUser("verifyuser", "Password123!", emailConfirmed: false);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = await _service.GenerateEmailVerificationTokenAsync(user.Id);

            // Act
            var result = await _service.VerifyEmailTokenAsync(user.Id, token);

            // Assert
            Assert.True(result);
            var dbToken = await _context.UserTokens.FirstOrDefaultAsync(t => t.UserId == user.Id && t.TokenType == "EmailVerification");
            Assert.NotNull(dbToken);
            Assert.True(dbToken.IsUsed);

            var updatedUser = await _context.Users.FirstAsync(u => u.Id == user.Id);
            Assert.True(updatedUser.EmailConfirmed);
        }

        [Fact]
        public async Task VerifyEmailToken_UsedToken_ReturnsFalse()
        {
            // Arrange
            var user = CreateTestUser("usedtoken", "Password123!", emailConfirmed: false);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = await _service.GenerateEmailVerificationTokenAsync(user.Id);
            await _service.VerifyEmailTokenAsync(user.Id, token); // Use it once

            // Act
            var result = await _service.VerifyEmailTokenAsync(user.Id, token); // Try again

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task VerifyEmailToken_InvalidToken_ReturnsFalse()
        {
            // Arrange
            var user = CreateTestUser("badtoken", "Password123!", emailConfirmed: false);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.VerifyEmailTokenAsync(user.Id, "invalid.token");

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}
