using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Zheng.Bll.Interfaces;

namespace Library.Server.Filters
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;

        public PermissionAuthorizationHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // 如果沒有指定權限，則直接通過（允許存取）
            if (requirement.Permissions == null || requirement.Permissions.Length == 0)
            {
                context.Succeed(requirement);
                return;
            }

            // 取得使用者 ID
            var userIdStr = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                return;
            }

            // 透過 PermissionService 取得使用者所有權限
            var userPermissions = await _permissionService.GetUserPermissionsAsync(userId);

            // 檢查是否有任何所需的權限（OR 邏輯）
            foreach (var requiredPermission in requirement.Permissions)
            {
                if (userPermissions.Contains(requiredPermission))
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            // 沒有找到權限，不做任何事（授權失敗）
        }
    }

}
