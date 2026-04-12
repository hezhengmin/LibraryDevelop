using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Zheng.Bll.Interfaces;
using Zheng.Mdl.Models.InputModel;
using Library.Server.Authorization;
using Library.Server.Filters;
using Library.Server.Services;

namespace Library.Server.Controllers
{
    /// <summary>
    /// 系統選單管理 API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SysMenuController : ApiController
    {
        private readonly ISysMenuService _sysMenuService;

        public SysMenuController(ISysMenuService sysMenuService, ICurrentUser currentUser) 
            : base(currentUser)
        {
            _sysMenuService = sysMenuService;
        }

        /// <summary>
        /// 根據ID取得系統選單詳細資料
        /// </summary>
        /// <param name="id">選單ID</param>
        /// <returns>系統選單詳細資料</returns>
        [HttpGet("{id}")]
        [Permission(Permissions.System.Menu)]
        public async Task<IActionResult> GetSysMenuById(long id)
        {
            try
            {
                var menu = await _sysMenuService.GetSysMenuByIdAsync(id);

                if (menu == null)
                {
                    AddError("找不到指定的選單");
                    return CustomResponse();
                }

                return CustomResponse(menu);
            }
            catch (Exception ex)
            {
                AddError($"取得選單資料失敗：{ex.Message}");
                return CustomResponse();
            }
        }

        /// <summary>
        /// 新增系統選單
        /// </summary>
        /// <param name="inputModel">新增資料</param>
        /// <returns>新增成功的選單資料</returns>
        [HttpPost]
        [Permission(Permissions.System.MenuCreate)]
        public async Task<IActionResult> CreateSysMenu([FromBody] SysMenuCreateInputModel inputModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            try
            {
                var result = await _sysMenuService.CreateSysMenuAsync(inputModel, CurrentUserId);
                return CustomResponse(result);
            }
            catch (InvalidOperationException ex)
            {
                AddError(ex.Message);
                return CustomResponse();
            }
            catch (Exception ex)
            {
                AddError($"新增選單失敗：{ex.Message}");
                return CustomResponse();
            }
        }

        /// <summary>
        /// 更新系統選單
        /// </summary>
        /// <param name="id">選單ID</param>
        /// <param name="inputModel">更新資料</param>
        /// <returns>是否更新成功</returns>
        [HttpPut("{id}")]
        [Permission(Permissions.System.MenuUpdate)]
        public async Task<IActionResult> UpdateSysMenu(long id, [FromBody] SysMenuUpdateInputModel inputModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            try
            {
                var success = await _sysMenuService.UpdateSysMenuAsync(id, inputModel, CurrentUserId);

                if (!success)
                {
                    AddError("找不到指定的選單");
                    return CustomResponse();
                }

                return CustomResponse("選單更新成功");
            }
            catch (InvalidOperationException ex)
            {
                AddError(ex.Message);
                return CustomResponse();
            }
            catch (Exception ex)
            {
                AddError($"更新選單失敗：{ex.Message}");
                return CustomResponse();
            }
        }

        /// <summary>
        /// 刪除系統選單
        /// </summary>
        /// <param name="id">選單ID</param>
        /// <returns>是否刪除成功</returns>
        [HttpDelete("{id}")]
        [Permission(Permissions.System.MenuDelete)]
        public async Task<IActionResult> DeleteSysMenu(long id)
        {
            try
            {
                var success = await _sysMenuService.DeleteSysMenuAsync(id);

                if (!success)
                {
                    AddError("找不到指定的選單");
                    return CustomResponse();
                }

                return CustomResponse("選單刪除成功");
            }
            catch (InvalidOperationException ex)
            {
                AddError(ex.Message);
                return CustomResponse();
            }
            catch (Exception ex)
            {
                AddError($"刪除選單失敗：{ex.Message}");
                return CustomResponse();
            }
        }

        /// <summary>
        /// 取得選單樹狀結構
        /// </summary>
        /// <returns>選單樹狀結構</returns>
        [HttpGet("tree")]
        public async Task<IActionResult> GetMenuTree()
        {
            try
            {
                var result = await _sysMenuService.GetMenuTreeAsync();
                return CustomResponse(result);
            }
            catch (Exception ex)
            {
                AddError($"取得選單樹狀結構失敗：{ex.Message}");
                return CustomResponse();
            }
        }

        /// <summary>
        /// 取得當前用戶的導航選單（用於左側選單顯示）
        /// </summary>
        /// <returns>用戶可訪問的選單列表</returns>
        [HttpGet("navigation")]
        public async Task<IActionResult> GetUserNavigationMenus()
        {
            try
            {
                var roleNames = CurrentUserRoleIds;
                var result = await _sysMenuService.GetUserNavigationMenusAsync(roleNames);
                return CustomResponse(result);
            }
            catch (Exception ex)
            {
                AddError($"取得導航選單失敗：{ex.Message}");
                return CustomResponse();
            }
        }
    }
}
