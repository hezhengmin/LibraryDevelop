using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zheng.Bll.Interfaces;
using Zheng.Infra.Data.Models;
using Zheng.Mdl.Models.Dto;
using Zheng.Mdl.Models.InputModel;

namespace Zheng.Bll.Services
{
    /// <summary>
    /// 系統選單服務實作
    /// </summary>
    public class SysMenuService : ISysMenuService
    {
        private readonly IGenericRepository<SysMenu> _sysMenuRepository;
        private readonly LibraryContext _context;

        public SysMenuService(IGenericRepository<SysMenu> sysMenuRepository, LibraryContext context)
        {
            _sysMenuRepository = sysMenuRepository;
            _context = context;
        }

        /// <summary>
        /// 根據ID取得系統選單詳細資料
        /// </summary>
        public async Task<SysMenuDto> GetSysMenuByIdAsync(long id)
        {
            var menu = await _sysMenuRepository.GetAll()
                .Where(m => m.Id == id)
                .Select(m => new SysMenuDto
                {
                    Id = m.Id,
                    ParentModuleId = m.ParentModuleId,
                    ModuleNo = m.ModuleNo,
                    Component = m.Component,
                    Hidden = m.Hidden,
                    Icon = m.Icon,
                    IsMenu = m.IsMenu,
                    IsOpen = m.IsOpen,
                    Levels = m.Levels,
                    Name = m.Name,
                    SortOrder = m.SortOrder,
                    Status = m.Status,
                    Tips = m.Tips,
                    Url = m.Url,
                    CreatedAt = m.CreatedAt,
                    CreatedBy = m.CreatedBy,
                    UpdatedAt = m.UpdatedAt,
                    UpdatedBy = m.UpdatedBy
                })
                .FirstOrDefaultAsync();

            return menu;
        }

        /// <summary>
        /// 新增系統選單
        /// </summary>
        public async Task<SysMenuDto> CreateSysMenuAsync(SysMenuCreateInputModel inputModel, Guid userId)
        {
            // 檢查選單編號是否已存在
            var existingMenu = await _sysMenuRepository.GetAll()
                .FirstOrDefaultAsync(m => m.ModuleNo == inputModel.ModuleNo);

            if (existingMenu != null)
            {
                throw new InvalidOperationException("選單編號已存在");
            }

            var newMenu = new SysMenu
            {
                ParentModuleId = inputModel.ParentModuleId,
                ModuleNo = inputModel.ModuleNo,
                Component = inputModel.Component,
                Hidden = inputModel.Hidden,
                Icon = inputModel.Icon,
                IsMenu = inputModel.IsMenu,
                IsOpen = inputModel.IsOpen,
                Levels = inputModel.Levels,
                Name = inputModel.Name,
                SortOrder = inputModel.SortOrder,
                Status = inputModel.Status,
                Tips = inputModel.Tips,
                Url = inputModel.Url,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };

            _sysMenuRepository.Add(newMenu);
            await _context.SaveChangesAsync();

            return new SysMenuDto
            {
                Id = newMenu.Id,
                ParentModuleId = newMenu.ParentModuleId,
                ModuleNo = newMenu.ModuleNo,
                Component = newMenu.Component,
                Hidden = newMenu.Hidden,
                Icon = newMenu.Icon,
                IsMenu = newMenu.IsMenu,
                IsOpen = newMenu.IsOpen,
                Levels = newMenu.Levels,
                Name = newMenu.Name,
                SortOrder = newMenu.SortOrder,
                Status = newMenu.Status,
                Tips = newMenu.Tips,
                Url = newMenu.Url,
                CreatedAt = newMenu.CreatedAt,
                CreatedBy = newMenu.CreatedBy,
                UpdatedAt = newMenu.UpdatedAt,
                UpdatedBy = newMenu.UpdatedBy
            };
        }

        /// <summary>
        /// 更新系統選單
        /// </summary>
        public async Task<bool> UpdateSysMenuAsync(long id, SysMenuUpdateInputModel inputModel, Guid userId)
        {
            var menu = await _sysMenuRepository.GetAll()
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (menu == null)
            {
                return false;
            }

            // 如果更新選單編號，檢查是否與其他選單重複
            if (!string.IsNullOrEmpty(inputModel.ModuleNo) && inputModel.ModuleNo != menu.ModuleNo)
            {
                var existingMenu = await _sysMenuRepository.GetAll()
                    .FirstOrDefaultAsync(m => m.ModuleNo == inputModel.ModuleNo && m.Id != id);

                if (existingMenu != null)
                {
                    throw new InvalidOperationException("選單編號已存在");
                }
                menu.ModuleNo = inputModel.ModuleNo;
            }

            // 更新各欄位
            if (inputModel.ParentModuleId.HasValue)
                menu.ParentModuleId = inputModel.ParentModuleId.Value;

            if (inputModel.Component != null)
                menu.Component = inputModel.Component;

            if (inputModel.Hidden.HasValue)
                menu.Hidden = inputModel.Hidden;

            if (inputModel.Icon != null)
                menu.Icon = inputModel.Icon;

            if (inputModel.IsMenu.HasValue)
                menu.IsMenu = inputModel.IsMenu.Value;

            if (inputModel.IsOpen.HasValue)
                menu.IsOpen = inputModel.IsOpen;

            if (inputModel.Levels.HasValue)
                menu.Levels = inputModel.Levels.Value;

            if (inputModel.Name != null)
                menu.Name = inputModel.Name;

            if (inputModel.SortOrder.HasValue)
                menu.SortOrder = inputModel.SortOrder.Value;

            if (inputModel.Status.HasValue)
                menu.Status = inputModel.Status.Value;

            if (inputModel.Tips != null)
                menu.Tips = inputModel.Tips;

            if (inputModel.Url != null)
                menu.Url = inputModel.Url;

            menu.UpdatedAt = DateTime.Now;
            menu.UpdatedBy = userId;

            _sysMenuRepository.Update(menu);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// 刪除系統選單
        /// </summary>
        public async Task<bool> DeleteSysMenuAsync(long id)
        {
            // 檢查是否有子選單
            var hasChildren = await _sysMenuRepository.GetAll()
                .AnyAsync(m => m.ParentModuleId == id);

            if (hasChildren)
            {
                throw new InvalidOperationException("此選單下還有子選單，無法刪除");
            }

            var menu = await _sysMenuRepository.GetAll()
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (menu == null)
            {
                return false;
            }

            _sysMenuRepository.Delete(menu);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// 取得所有選單（樹狀結構）- 包含選單和按鈕
        /// </summary>
        public async Task<List<SysMenuDto>> GetMenuTreeAsync()
        {
            var allMenus = await _sysMenuRepository.GetAll()
                // 移除 IsMenu 篩選，顯示所有項目（包含選單和按鈕）
                .OrderBy(m => m.Levels)
                .ThenBy(m => m.SortOrder)
                .Select(m => new SysMenuDto
                {
                    Id = m.Id,
                    ParentModuleId = m.ParentModuleId,
                    ModuleNo = m.ModuleNo,
                    Component = m.Component,
                    Hidden = m.Hidden,
                    Icon = m.Icon,
                    IsMenu = m.IsMenu,
                    IsOpen = m.IsOpen,
                    Levels = m.Levels,
                    Name = m.Name,
                    SortOrder = m.SortOrder,
                    Status = m.Status,
                    Tips = m.Tips,
                    Url = m.Url,
                    CreatedAt = m.CreatedAt,
                    CreatedBy = m.CreatedBy,
                    UpdatedAt = m.UpdatedAt,
                    UpdatedBy = m.UpdatedBy
                })
                .ToListAsync();

            // 建立樹狀結構
            var tree = BuildMenuTree(allMenus, 0);
            return tree;
        }

        /// <summary>
        /// 取得當前用戶的導航選單（用於左側選單顯示）
        /// </summary>
        public async Task<List<SysMenuDto>> GetUserNavigationMenusAsync(IEnumerable<string> roleNames)
        {
            // 透過角色名稱查詢角色擁有的權限（ModuleNo）
            var roleNameList = roleNames.ToList();
            var permittedModuleNos = await _context.Roles
                .Where(r => roleNameList.Contains(r.Name))
                .SelectMany(r => r.RolePermissions)
                .Select(rp => rp.Permission)
                .Distinct()
                .ToListAsync();

            // 取得所有啟用且非隱藏的選單項目（只取選單，不取按鈕）
            var allMenus = await _sysMenuRepository.GetAll()
                .Where(m => m.Status && (m.Hidden == false || m.Hidden == null) && m.IsMenu)
                .OrderBy(m => m.Levels)
                .ThenBy(m => m.SortOrder)
                .Select(m => new SysMenuDto
                {
                    Id = m.Id,
                    ParentModuleId = m.ParentModuleId,
                    ModuleNo = m.ModuleNo,
                    Component = m.Component,
                    Hidden = m.Hidden,
                    Icon = m.Icon,
                    IsMenu = m.IsMenu,
                    IsOpen = m.IsOpen,
                    Levels = m.Levels,
                    Name = m.Name,
                    SortOrder = m.SortOrder,
                    Status = m.Status,
                    Tips = m.Tips,
                    Url = m.Url
                })
                .ToListAsync();

            // 過濾：保留使用者有權限的選單，以及其父選單
            var permittedMenuIds = new HashSet<long>();
            var menuDict = allMenus.ToDictionary(m => m.Id);

            foreach (var menu in allMenus)
            {
                if (permittedModuleNos.Contains(menu.ModuleNo))
                {
                    // 標記此選單及其所有祖先選單
                    var current = menu;
                    while (current != null)
                    {
                        permittedMenuIds.Add(current.Id);
                        menuDict.TryGetValue(current.ParentModuleId, out current);
                    }
                }
            }

            var filteredMenus = allMenus.Where(m => permittedMenuIds.Contains(m.Id)).ToList();

            // 建立樹狀結構
            var tree = BuildMenuTree(filteredMenus, 0);
            return tree;
        }

        /// <summary>
        /// 建立選單樹狀結構的輔助方法
        /// </summary>
        private List<SysMenuDto> BuildMenuTree(List<SysMenuDto> allMenus, long parentId)
        {
            return allMenus
                .Where(m => m.ParentModuleId == parentId)
                .Select(m =>
                {
                    m.Children = BuildMenuTree(allMenus, m.Id);
                    return m;
                })
                .ToList();
        }
    }
}
