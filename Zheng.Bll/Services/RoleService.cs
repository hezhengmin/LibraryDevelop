using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zheng.Bll.Interfaces;
using Zheng.Infra.Data.Models;
using Zheng.Mdl.Models;

namespace Zheng.Bll.Services
{
    public class RoleService : IRoleService
    {
        private readonly LibraryContext _context;

        public RoleService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<RoleDto>> GetRolesAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Roles.AsQueryable();
            var totalCount = await query.CountAsync();

            var roles = await query
                .OrderBy(r => r.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    UserCount = r.UserRoles.Count
                })
                .ToListAsync();

            return new PagedResult<RoleDto>(roles, totalCount, page, pageSize);
        }

        public async Task<RoleDetailDto?> GetRoleWithPermissionsAsync(Guid id)
        {
            return await _context.Roles
                .Where(r => r.Id == id)
                .Select(r => new RoleDetailDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Permissions = r.RolePermissions.Select(rp => rp.Permission).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Result> CreateRoleAsync(CreateRoleInput input)
        {
            var normalizedName = input.Name.ToUpperInvariant();
            if (await _context.Roles.AnyAsync(r => r.NormalizedName == normalizedName))
            {
                return Result.Failure(new[] { "角色名稱已存在" });
            }

            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = input.Name,
                NormalizedName = normalizedName,
                Description = input.Description,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Roles.Add(role);

            if (input.Permissions != null && input.Permissions.Any())
            {
                foreach (var permission in input.Permissions)
                {
                    _context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = role.Id,
                        Permission = permission
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Result.Success("角色建立成功", new { role.Id, role.Name });
        }

        public async Task<Result> UpdateRoleAsync(Guid id, UpdateRoleInput input)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (role == null)
            {
                return Result.Failure(new[] { "找不到指定的角色" });
            }

            // Check unique name if changed
            var normalizedName = input.Name.ToUpperInvariant();
            if (normalizedName != role.NormalizedName &&
                await _context.Roles.AnyAsync(r => r.NormalizedName == normalizedName && r.Id != id))
            {
                return Result.Failure(new[] { "角色名稱已存在" });
            }

            role.Name = input.Name;
            role.NormalizedName = normalizedName;
            role.Description = input.Description;
            role.UpdatedAt = DateTime.UtcNow;

            // Diff update permissions
            if (input.Permissions != null)
            {
                var existingPermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == id)
                    .ToListAsync();

                var toRemove = existingPermissions
                    .Where(ep => !input.Permissions.Contains(ep.Permission))
                    .ToList();
                _context.RolePermissions.RemoveRange(toRemove);

                var existingPermNames = existingPermissions.Select(ep => ep.Permission).ToHashSet();
                var toAdd = input.Permissions
                    .Where(p => !existingPermNames.Contains(p))
                    .Select(p => new RolePermission { RoleId = id, Permission = p });
                _context.RolePermissions.AddRange(toAdd);
            }

            await _context.SaveChangesAsync();
            return Result.Success("角色更新成功", new { role.Id, role.Name });
        }

        public async Task<Result> DeleteRoleAsync(Guid id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (role == null)
            {
                return Result.Failure(new[] { "找不到指定的角色" });
            }

            // Check if any users are assigned to this role
            var userCount = await _context.UserRoles.CountAsync(ur => ur.RoleId == id);
            if (userCount > 0)
            {
                return Result.Failure(new[] { $"此角色尚有 {userCount} 位使用者指派，無法刪除" });
            }

            // Remove permissions first
            var permissions = await _context.RolePermissions.Where(rp => rp.RoleId == id).ToListAsync();
            _context.RolePermissions.RemoveRange(permissions);

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return Result.Success("角色刪除成功");
        }

        public async Task<IList<string>> GetPermissionsAsync(Guid roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permission)
                .ToListAsync();
        }

        public async Task<IList<UserDto>> GetUsersInRoleAsync(Guid roleId)
        {
            return await _context.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => new UserDto
                {
                    Id = ur.User.Id,
                    UserName = ur.User.UserName,
                    Email = ur.User.Email,
                    EmailConfirmed = ur.User.EmailConfirmed,
                    PhoneNumber = ur.User.PhoneNumber,
                    LockoutEnd = ur.User.LockoutEnd,
                    LockoutEnabled = ur.User.LockoutEnabled,
                    AccessFailedCount = ur.User.AccessFailedCount,
                    IsActive = ur.User.IsActive
                })
                .ToListAsync();
        }
    }
}
