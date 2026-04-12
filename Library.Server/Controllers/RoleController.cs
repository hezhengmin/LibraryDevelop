using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zheng.Mdl.Models.InputModel;
using Zheng.Mdl.Models.Dto;
using Zheng.Bll.Interfaces;
using Library.Server.Authorization;
using Library.Server.Filters;
using Library.Server.Services;

namespace Library.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : ApiController
    {
        private readonly IRoleService _roleService;
        private readonly ISysMenuService _sysMenuService;

        public RoleController(
            IRoleService roleService,
            ISysMenuService sysMenuService,
            ICurrentUser currentUser) 
            : base(currentUser)
        {
            _roleService = roleService;
            _sysMenuService = sysMenuService;
        }

        /// <summary>
        /// 取得所有角色列表（支援分頁）
        /// </summary>
        [HttpGet]
        [Permission(Permissions.Roles.View)]
        public async Task<IActionResult> GetRoles([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _roleService.GetRolesAsync(pageNumber, pageSize);
            return CustomResponse("取得角色列表成功", pagedResult);
        }

        /// <summary>
        /// 取得單一角色詳情
        /// </summary>
        [HttpGet("{id}")]
        [Permission(Permissions.Roles.View)]
        public async Task<IActionResult> GetRole(Guid id)
        {
            var role = await _roleService.GetRoleWithPermissionsAsync(id);
            if (role == null)
            {
                AddError("找不到指定的角色");
                return CustomResponse();
            }

            return CustomResponse("取得角色詳情成功", role);
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        [HttpPost]
        [Permission(Permissions.Roles.Create)]
        public async Task<IActionResult> CreateRole([FromBody] RoleInputModel model)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _roleService.CreateRoleAsync(new CreateRoleInput
            {
                Name = model.Name,
                Description = null,
                Permissions = model.Permissions
            });

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    AddError(error);
                }
                return CustomResponse();
            }

            return CustomResponse("角色建立成功", result.Data);
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        [HttpPut("{id}")]
        [Permission(Permissions.Roles.Edit)]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] RoleInputModel model)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _roleService.UpdateRoleAsync(id, new UpdateRoleInput
            {
                Name = model.Name,
                Description = null,
                Permissions = model.Permissions
            });

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    AddError(error);
                }
                return CustomResponse();
            }

            return CustomResponse("角色更新成功", result.Data);
        }

        /// <summary>
        /// 刪除角色
        /// </summary>
        [HttpDelete("{id}")]
        [Permission(Permissions.Roles.Delete)]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    AddError(error);
                }
                return CustomResponse();
            }

            return CustomResponse("角色刪除成功");
        }

        /// <summary>
        /// 取得所有可用的權限列表（從 SysMenu 自動生成樹狀結構）
        /// </summary>
        [HttpGet("permissions")]
        public async Task<IActionResult> GetAvailablePermissions()
        {
            var menuTree = await _sysMenuService.GetMenuTreeAsync();
            var permissionTree = MapToPermissionTree(menuTree);
            return CustomResponse("取得權限列表成功", permissionTree);
        }

        private static List<object> MapToPermissionTree(List<SysMenuDto> menus)
        {
            return menus.Select(m => (object)new
            {
                id = m.Id,
                label = m.Name,
                moduleNo = m.ModuleNo,
                isMenu = m.IsMenu,
                children = m.Children != null && m.Children.Any()
                    ? MapToPermissionTree(m.Children)
                    : new List<object>()
            }).ToList();
        }

        /// <summary>
        /// 取得指定角色的使用者列表
        /// </summary>
        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetRoleUsers(Guid id)
        {
            var users = await _roleService.GetUsersInRoleAsync(id);
            return CustomResponse("取得角色使用者列表成功", users);
        }
    }
}
