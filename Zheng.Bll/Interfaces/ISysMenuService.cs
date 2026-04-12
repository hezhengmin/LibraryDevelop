using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zheng.Mdl.Models.Dto;
using Zheng.Mdl.Models.InputModel;

namespace Zheng.Bll.Interfaces
{
    /// <summary>
    /// 系統選單服務介面
    /// </summary>
    public interface ISysMenuService
    {
        /// <summary>
        /// 根據ID取得系統選單詳細資料
        /// </summary>
        /// <param name="id">選單ID</param>
        /// <returns>系統選單詳細資料</returns>
        Task<SysMenuDto> GetSysMenuByIdAsync(long id);

        /// <summary>
        /// 新增系統選單
        /// </summary>
        /// <param name="inputModel">新增資料</param>
        /// <param name="userId">操作者ID</param>
        /// <returns>新增成功的選單資料</returns>
        Task<SysMenuDto> CreateSysMenuAsync(SysMenuCreateInputModel inputModel, Guid userId);

        /// <summary>
        /// 更新系統選單
        /// </summary>
        /// <param name="id">選單ID</param>
        /// <param name="inputModel">更新資料</param>
        /// <param name="userId">操作者ID</param>
        /// <returns>是否更新成功</returns>
        Task<bool> UpdateSysMenuAsync(long id, SysMenuUpdateInputModel inputModel, Guid userId);

        /// <summary>
        /// 刪除系統選單
        /// </summary>
        /// <param name="id">選單ID</param>
        /// <returns>是否刪除成功</returns>
        Task<bool> DeleteSysMenuAsync(long id);

        /// <summary>
        /// 取得所有啟用的選單（樹狀結構）
        /// </summary>
        /// <returns>選單樹狀結構</returns>
        Task<List<SysMenuDto>> GetMenuTreeAsync();

        /// <summary>
        /// 取得當前用戶的導航選單（用於左側選單顯示）
        /// </summary>
        /// <param name="roleNames">當前用戶的角色名稱列表</param>
        /// <returns>用戶可訪問的選單列表</returns>
        Task<List<SysMenuDto>> GetUserNavigationMenusAsync(IEnumerable<string> roleNames);
    }
}
