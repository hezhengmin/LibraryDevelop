using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zheng.Bll.Interfaces;
using Library.Server.Services;

namespace Library.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalLoginController : ApiController
    {
        private readonly IJwtService _jwtService;
        private readonly IExternalLoginService _externalLoginService;
        private readonly IConfiguration _configuration;

        public ExternalLoginController(
            IJwtService jwtService,
            IExternalLoginService externalLoginService,
            ICurrentUser currentUser,
            IConfiguration configuration)
            : base(currentUser)
        {
            _jwtService = jwtService;
            _externalLoginService = externalLoginService;
            _configuration = configuration;
        }

        /// <summary>
        /// 發起外部登入 — 導向社群供應商授權頁面
        /// </summary>
        /// <param name="provider">供應商名稱 (Google / Facebook)</param>
        [HttpGet("Challenge/{provider}")]
        public IActionResult Challenge(string provider)
        {
            var allowedProviders = new[] { "Google", "Facebook" };
            if (!allowedProviders.Contains(provider, StringComparer.OrdinalIgnoreCase))
            {
                AddError("不支援的登入供應商");
                return CustomResponse();
            }

            var callbackUrl = Url.Action(nameof(Callback), "ExternalLogin",
                new { provider }, Request.Scheme);

            var properties = new AuthenticationProperties
            {
                RedirectUri = callbackUrl,
                Items = { { "LoginProvider", provider } }
            };

            return Challenge(properties, provider);
        }

        /// <summary>
        /// OAuth 回呼端點 — 供應商授權完成後呼叫此端點
        /// </summary>
        [HttpGet("Callback/{provider}")]
        public async Task<IActionResult> Callback(string provider)
        {
            var result = await HttpContext.AuthenticateAsync("ExternalLogin");
            if (!result.Succeeded || result.Principal == null)
            {
                return RedirectToFrontend(error: "授權失敗，請重新登入");
            }

            var claims = result.Principal.Claims.ToList();
            var providerKey = claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(providerKey))
            {
                return RedirectToFrontend(error: "無法取得社群帳號資訊");
            }

            // Clean up the external cookie
            await HttpContext.SignOutAsync("ExternalLogin");

            var loginResult = await _externalLoginService.ProcessExternalLoginAsync(
                provider, providerKey, email, name);

            if (!loginResult.Succeeded || loginResult.User == null)
            {
                return RedirectToFrontend(error: loginResult.Error ?? "社群登入失敗");
            }

            // Generate JWT tokens (same as password login)
            var accessToken = await _jwtService.GenerateAccessTokenAsync(loginResult.User);
            var refreshToken = _jwtService.GenerateRefreshToken();
            await _jwtService.SaveTokenAsync(loginResult.User.Id, accessToken, refreshToken);

            return RedirectToFrontend(
                accessToken: accessToken,
                refreshToken: refreshToken,
                userName: loginResult.User.UserName);
        }

        /// <summary>
        /// 建立前端回呼 URL 並重導
        /// </summary>
        private IActionResult RedirectToFrontend(
            string? accessToken = null,
            string? refreshToken = null,
            string? userName = null,
            string? error = null)
        {
            var frontendBaseUrl = _configuration.GetValue<string>("FrontendBaseUrl") ?? "";
            var callbackPath = "/oauth/callback";

            if (!string.IsNullOrEmpty(error))
            {
                var errorUrl = $"{frontendBaseUrl}{callbackPath}?error={Uri.EscapeDataString(error)}";
                return Redirect(errorUrl);
            }

            var successUrl = $"{frontendBaseUrl}{callbackPath}" +
                $"?accessToken={Uri.EscapeDataString(accessToken!)}" +
                $"&refreshToken={Uri.EscapeDataString(refreshToken!)}" +
                $"&userName={Uri.EscapeDataString(userName!)}";
            return Redirect(successUrl);
        }
    }
}
