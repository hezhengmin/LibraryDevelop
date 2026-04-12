using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Zheng.Bll.Interfaces;
using Zheng.Mdl.Models;

namespace Zheng.Bll.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _memoryCache;

        public RegistrationService(
            IUserService userService,
            IAuthService authService,
            IEmailSender emailSender,
            IMemoryCache memoryCache)
        {
            _userService = userService;
            _authService = authService;
            _emailSender = emailSender;
            _memoryCache = memoryCache;
        }

        public async Task<Result> RegisterAsync(string userName, string email, string password, string confirmationUrlBase)
        {
            var createResult = await _userService.CreateUserAsync(new CreateUserInput
            {
                UserName = userName,
                Email = email,
                Password = password,
                EmailConfirmed = false
            });

            if (!createResult.Succeeded)
                return createResult;

            // Extract userId from result data
            var userData = createResult.Data as dynamic;
            Guid userId = userData?.Id ?? Guid.Empty;

            // Generate email verification token
            var token = await _authService.GenerateEmailVerificationTokenAsync(userId);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var confirmationUrl = $"{confirmationUrlBase}?userId={userId}&token={encodedToken}";

            var htmlBody = $@"
                <h2>歡迎加入圖書管理系統</h2>
                <p>請點擊下方連結完成電子郵件驗證：</p>
                <p><a href='{confirmationUrl}'>驗證電子郵件</a></p>
                <p>此連結將在 24 小時後失效。</p>
                <p>若您未曾註冊此帳號，請忽略此信件。</p>";

            await _emailSender.SendEmailAsync(email, "電子郵件驗證 - 圖書管理系統", htmlBody);

            return createResult;
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            if (!Guid.TryParse(userId, out var userGuid))
                return false;

            var user = await _userService.FindByIdAsync(userGuid);
            if (user == null)
                return false;

            if (user.EmailConfirmed)
                return true;

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            return await _authService.VerifyEmailTokenAsync(userGuid, decodedToken);
        }

        public async Task<bool> ResendVerificationAsync(string email, string confirmationUrlBase)
        {
            // Rate limiting: 1 request per minute per email
            var cacheKey = $"resend_verify_{email.ToLowerInvariant().GetHashCode()}";
            if (_memoryCache.TryGetValue(cacheKey, out _))
                return false;

            var user = await _userService.FindByEmailAsync(email);
            if (user == null || user.EmailConfirmed)
            {
                // Don't reveal whether email exists (prevent account enumeration)
                _memoryCache.Set(cacheKey, true, TimeSpan.FromSeconds(60));
                return true;
            }

            var token = await _authService.GenerateEmailVerificationTokenAsync(user.Id);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationUrl = $"{confirmationUrlBase}?userId={user.Id}&token={encodedToken}";

            var htmlBody = $@"
                <h2>電子郵件驗證 - 圖書管理系統</h2>
                <p>請點擊下方連結完成電子郵件驗證：</p>
                <p><a href='{confirmationUrl}'>驗證電子郵件</a></p>
                <p>此連結將在 24 小時後失效。</p>
                <p>若您未曾註冊此帳號，請忽略此信件。</p>";

            await _emailSender.SendEmailAsync(email, "重新發送驗證信 - 圖書管理系統", htmlBody);

            _memoryCache.Set(cacheKey, true, TimeSpan.FromSeconds(60));
            return true;
        }
    }
}
