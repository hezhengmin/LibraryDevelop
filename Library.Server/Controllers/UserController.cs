using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zheng.Mdl.Models.InputModel;
using Library.Server.Authorization;
using Library.Server.Filters;
using Library.Server.Services;
using Zheng.Bll.Interfaces;

namespace Library.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ApiController
    {
        private readonly IUserService _userService;

        public UserController(
            IUserService userService,
            ICurrentUser currentUser)
            : base(currentUser)
        {
            _userService = userService;
        }

        /// <summary>
        /// 取得所有使用者列表（支援分頁）
        /// </summary>
        [HttpGet]
        [Permission(Permissions.Users.View)]
        public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? keyword = null)
        {
            var pagedResult = await _userService.GetUsersAsync(pageNumber, pageSize, keyword);
            return CustomResponse("取得使用者列表成功", pagedResult);
        }

        /// <summary>
        /// 取得單一使用者詳情
        /// </summary>
        [HttpGet("{id}")]
        [Permission(Permissions.Users.View)]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _userService.GetUserWithRolesAsync(id);
            if (user == null)
            {
                AddError("找不到指定的使用者");
                return CustomResponse();
            }

            return CustomResponse("取得使用者詳情成功", user);
        }

        /// <summary>
        /// 新增使用者
        /// </summary>
        [HttpPost]
        [Permission(Permissions.Users.Create)]
        public async Task<IActionResult> CreateUser([FromBody] UserInputModel model)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _userService.CreateUserAsync(new CreateUserInput
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = model.EmailConfirmed,
                Roles = model.Roles
            });

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    AddError(error);
                }
                return CustomResponse();
            }

            return CustomResponse(result.Message, result.Data);
        }

        /// <summary>
        /// 更新使用者
        /// </summary>
        [HttpPut("{id}")]
        [Permission(Permissions.Users.Edit)]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateInputModel model)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _userService.UpdateUserAsync(id, new UpdateUserInput
            {
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = model.EmailConfirmed,
                Roles = model.Roles
            });

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    AddError(error);
                }
                return CustomResponse();
            }

            return CustomResponse(result.Message, result.Data);
        }

        /// <summary>
        /// 刪除使用者
        /// </summary>
        [HttpDelete("{id}")]
        [Permission(Permissions.Users.Delete)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    AddError(error);
                }
                return CustomResponse();
            }

            return CustomResponse(result.Message);
        }

        /// <summary>
        /// 重設密碼
        /// </summary>
        [HttpPost("{id}/reset-password")]
        [Permission(Permissions.Users.ResetPassword)]
        public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordInputModel model)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _userService.ResetPasswordAsync(id, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    AddError(error);
                }
                return CustomResponse();
            }

            return CustomResponse(result.Message);
        }

        /// <summary>
        /// 鎖定使用者
        /// </summary>
        [HttpPost("{id}/lock")]
        public async Task<IActionResult> LockUser(Guid id)
        {
            var result = await _userService.LockUserAsync(id);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    AddError(error);
                }
                return CustomResponse();
            }

            return CustomResponse(result.Message);
        }

        /// <summary>
        /// 解鎖使用者
        /// </summary>
        [HttpPost("{id}/unlock")]
        public async Task<IActionResult> UnlockUser(Guid id)
        {
            var result = await _userService.UnlockUserAsync(id);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    AddError(error);
                }
                return CustomResponse();
            }

            return CustomResponse(result.Message);
        }

        /// <summary>
        /// 取得使用者的角色
        /// </summary>
        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetUserRoles(Guid id)
        {
            var user = await _userService.FindByIdAsync(id);
            if (user == null)
            {
                AddError("找不到指定的使用者");
                return CustomResponse();
            }

            var roles = await _userService.GetRolesAsync(id);
            return CustomResponse("取得使用者角色成功", roles);
        }

        /// <summary>
        /// 設定使用者的角色
        /// </summary>
        [HttpPost("{id}/roles")]
        public async Task<IActionResult> SetUserRoles(Guid id, [FromBody] SetRolesInputModel model)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _userService.SetRolesAsync(id, model.Roles);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    AddError(error);
                }
                return CustomResponse();
            }

            return CustomResponse(result.Message);
        }
    }
}
