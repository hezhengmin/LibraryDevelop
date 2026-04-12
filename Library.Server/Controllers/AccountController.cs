using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Zheng.Mdl.Models.InputModel;
using Zheng.Bll.Interfaces;
using Library.Server.Services;

namespace Library.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ApiController
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        private readonly IRegistrationService _registrationService;
        private readonly IUserService _userService;

        public AccountController(
            IAuthService authService,
            IJwtService jwtService,
            ICurrentUser currentUser,
            IRegistrationService registrationService,
            IUserService userService) 
            : base(currentUser)
        {
            _authService = authService;
            _jwtService = jwtService;
            _registrationService = registrationService;
            _userService = userService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginInputModel loginInputModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var authResult = await _authService.ValidatePasswordAsync(loginInputModel.UserName, loginInputModel.Password);
            if (authResult.Succeeded && authResult.User != null)
            {
                // Generate JWT tokens
                var accessToken = await _jwtService.GenerateAccessTokenAsync(authResult.User);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // Save tokens to database
                await _jwtService.SaveTokenAsync(authResult.User.Id, accessToken, refreshToken);

                // Return tokens in response
                return CustomResponse("登入成功", new
                {
                    accessToken,
                    refreshToken,
                    userName = authResult.User.UserName
                });
            }

            if (authResult.Error == "請先完成電子郵件驗證")
            {
                AddError("請先完成電子郵件驗證");
                return BadRequest(Zheng.Mdl.Models.Result.Failure(
                    new[] { "請先完成電子郵件驗證" },
                    data: new { requiresEmailVerification = true }));
            }

            AddError(authResult.Error ?? "登入失敗，請確認帳號密碼");
            return CustomResponse();
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                AddError("驗證連結無效");
                return CustomResponse();
            }

            if (!Guid.TryParse(userId, out var userGuid))
            {
                AddError("驗證連結無效");
                return CustomResponse();
            }

            var user = await _userService.FindByIdAsync(userGuid);
            if (user == null)
            {
                AddError("驗證連結無效或已過期");
                return CustomResponse();
            }

            if (user.EmailConfirmed)
            {
                return CustomResponse("帳號已驗證，請直接登入");
            }

            var confirmed = await _registrationService.ConfirmEmailAsync(userId, token);
            if (confirmed)
            {
                return CustomResponse("電子郵件驗證成功，請前往登入");
            }

            AddError("驗證連結無效或已過期");
            return CustomResponse();
        }

        [HttpPost("ResendVerification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationInputModel model)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var confirmationUrlBase = $"{Request.Scheme}://{Request.Host}/api/Account/ConfirmEmail";
            var sent = await _registrationService.ResendVerificationAsync(model.Email, confirmationUrlBase);

            if (!sent)
            {
                AddError("請稍後再試");
                return StatusCode(429, Zheng.Mdl.Models.Result.Failure(new[] { "請稍後再試" }));
            }

            // Always return success to prevent account enumeration
            return CustomResponse("若該信箱已註冊，驗證信已發送");
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterInputModel registerInputModel)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var confirmationUrlBase = $"{Request.Scheme}://{Request.Host}/api/Account/ConfirmEmail";
            var result = await _registrationService.RegisterAsync(
                registerInputModel.UserName,
                registerInputModel.Email,
                registerInputModel.Password,
                confirmationUrlBase);

            if (result.Succeeded)
            {
                return CustomResponse("註冊成功，請至信箱完成電子郵件驗證");
            }

            var errors = string.Join("；", result.Errors);
            AddError($"註冊失敗：{errors}");
            return CustomResponse();
        }

        [Authorize]
        [HttpGet("CurrentUser")]
        public IActionResult GetCurrentUser()
        {
            if (!_currentUser.IsAuthenticated)
                return Unauthorized();

            return CustomResponse(new
            {
                id = _currentUser.UserId,
                username = _currentUser.Account,
                email = _currentUser.Email,
                roles = _currentUser.RoleIds
            });
        }
    }
}
