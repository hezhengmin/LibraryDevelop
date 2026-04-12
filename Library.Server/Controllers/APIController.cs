using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Library.Server.Services;
using Zheng.Mdl.Models;

namespace Library.Server.Controllers
{
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        private readonly ICollection<string> _errors = new List<string>();
        protected readonly ICurrentUser _currentUser;

        protected ApiController(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        /// <summary>
        /// 取得當前登入用戶的 ID
        /// </summary>
        protected Guid CurrentUserId => _currentUser.UserId;

        /// <summary>
        /// 取得當前登入用戶的帳號
        /// </summary>
        protected string CurrentUserAccount => _currentUser.Account;

        /// <summary>
        /// 取得當前登入用戶的名稱
        /// </summary>
        protected string CurrentUserName => _currentUser.Name;

        /// <summary>
        /// 取得當前登入用戶的 Email
        /// </summary>
        protected string CurrentUserEmail => _currentUser.Email;

        /// <summary>
        /// 取得當前登入用戶的角色 ID（主要角色）
        /// </summary>
        protected string CurrentUserRoleId => _currentUser.RoleId;

        /// <summary>
        /// 取得當前登入用戶的所有角色 ID
        /// </summary>
        protected IEnumerable<string> CurrentUserRoleIds => _currentUser.RoleIds;

        /// <summary>
        /// 檢查當前用戶是否擁有指定角色
        /// </summary>
        protected bool HasRole(string roleId) => _currentUser.HasRole(roleId);

        /// <summary>
        /// 檢查當前用戶是否擁有任一指定角色
        /// </summary>
        protected bool HasAnyRole(params string[] roleIds) => _currentUser.HasAnyRole(roleIds);

        /// <summary>
        /// 檢查當前用戶是否擁有所有指定角色
        /// </summary>
        protected bool HasAllRoles(params string[] roleIds) => _currentUser.HasAllRoles(roleIds);

        /// <summary>
        /// 檢查當前用戶是否已認證
        /// </summary>
        protected bool IsAuthenticated => _currentUser.IsAuthenticated;

        protected ActionResult CustomResponse(object result = null)
        {
            if (IsOperationValid())
            {
                if (result is string message)
                    return Ok(Result.Success(message));
                if (result != null)
                    return Ok(Result.Success(data: result));
                return Ok(Result.Success());
            }
            return BadRequest(Result.Failure(_errors));
        }

        protected ActionResult CustomResponse(string message, object data)
        {
            if (IsOperationValid())
            {
                return Ok(Result.Success(message, data));
            }
            return BadRequest(Result.Failure(_errors, message, data));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(e => e.Errors);
            foreach (var error in errors)
            {
                AddError(error.ErrorMessage);
            }

            return CustomResponse();
        }

        protected bool IsOperationValid()
        {
            return !_errors.Any();
        }

        protected void AddError(string erro)
        {
            _errors.Add(erro);
        }

        protected void ClearErrors()
        {
            _errors.Clear();
        }
    }
}
