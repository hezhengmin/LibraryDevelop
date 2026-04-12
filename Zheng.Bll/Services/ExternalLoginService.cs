#nullable enable
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Zheng.Bll.Interfaces;
using Zheng.Infra.Data.Models;

namespace Zheng.Bll.Services
{
    public class ExternalLoginService : IExternalLoginService
    {
        private readonly LibraryContext _context;
        private readonly IAuthService _authService;

        public ExternalLoginService(LibraryContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<ExternalLoginResult> ProcessExternalLoginAsync(
            string loginProvider,
            string providerKey,
            string? email,
            string? name)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new ExternalLoginResult
                {
                    Succeeded = false,
                    Error = "社群帳號未提供電子郵件，請授權電子郵件權限後重試"
                };
            }

            // Check if user already has this external login
            var externalLogin = await _context.UserExternalLogins
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.LoginProvider == loginProvider && e.ProviderKey == providerKey);

            if (externalLogin != null)
            {
                // Existing external login — just sign in
                return new ExternalLoginResult
                {
                    Succeeded = true,
                    User = externalLogin.User,
                    IsNewUser = false
                };
            }

            // Check if a user with same email already exists
            var normalizedEmail = email.ToUpperInvariant();
            var userByEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail && u.IsActive);

            if (userByEmail != null)
            {
                // Link external login to existing account
                _context.UserExternalLogins.Add(new UserExternalLogin
                {
                    UserId = userByEmail.Id,
                    LoginProvider = loginProvider,
                    ProviderKey = providerKey,
                    ProviderDisplayName = loginProvider,
                    CreatedAt = DateTime.UtcNow
                });

                // Ensure email is confirmed (social provider already verified it)
                if (!userByEmail.EmailConfirmed)
                {
                    userByEmail.EmailConfirmed = true;
                    userByEmail.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return new ExternalLoginResult
                {
                    Succeeded = true,
                    User = userByEmail,
                    IsNewUser = false
                };
            }

            // Create new user from external login
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = email,
                NormalizedUserName = normalizedEmail,
                Email = email,
                NormalizedEmail = normalizedEmail,
                EmailConfirmed = true,
                PasswordHash = _authService.HashPassword(Guid.NewGuid().ToString()),
                PasswordHashVersion = 2,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            _context.Users.Add(newUser);

            _context.UserExternalLogins.Add(new UserExternalLogin
            {
                UserId = newUser.Id,
                LoginProvider = loginProvider,
                ProviderKey = providerKey,
                ProviderDisplayName = loginProvider,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return new ExternalLoginResult
            {
                Succeeded = true,
                User = newUser,
                IsNewUser = true
            };
        }
    }
}
