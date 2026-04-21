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
    public class UserService : IUserService
    {
        private readonly LibraryContext _context;
        private readonly IAuthService _authService;

        public UserService(LibraryContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<User?> FindByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }

        public async Task<User?> FindByNameAsync(string userName)
        {
            var normalized = userName.ToUpperInvariant();
            return await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalized && u.IsActive);
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            var normalized = email.ToUpperInvariant();
            return await _context.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalized && u.IsActive);
        }

        public async Task<PagedResult<UserDto>> GetUsersAsync(int page, int pageSize, string? keyword = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(u =>
                    u.UserName.Contains(keyword) ||
                    u.Email.Contains(keyword));
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    EmailConfirmed = u.EmailConfirmed,
                    PhoneNumber = u.PhoneNumber,
                    LockoutEnd = u.LockoutEnd,
                    LockoutEnabled = u.LockoutEnabled,
                    AccessFailedCount = u.AccessFailedCount,
                    IsActive = u.IsActive,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                })
                .ToListAsync();

            return new PagedResult<UserDto>(users, totalCount, page, pageSize);
        }

        public async Task<UserDto?> GetUserWithRolesAsync(Guid id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    EmailConfirmed = u.EmailConfirmed,
                    PhoneNumber = u.PhoneNumber,
                    LockoutEnd = u.LockoutEnd,
                    LockoutEnabled = u.LockoutEnabled,
                    AccessFailedCount = u.AccessFailedCount,
                    IsActive = u.IsActive,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Result> CreateUserAsync(CreateUserInput input)
        {
            // Check unique username
            var normalizedUserName = input.UserName.ToUpperInvariant();
            if (await _context.Users.AnyAsync(u => u.NormalizedUserName == normalizedUserName))
            {
                return Result.Failure(new[] { "使用者名稱已存在" });
            }

            // Check unique email
            var normalizedEmail = input.Email.ToUpperInvariant();
            if (await _context.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail))
            {
                return Result.Failure(new[] { "Email 已被使用" });
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = input.UserName,
                NormalizedUserName = normalizedUserName,
                Email = input.Email,
                NormalizedEmail = normalizedEmail,
                EmailConfirmed = input.EmailConfirmed,
                PasswordHash = _authService.HashPassword(input.Password),
                PasswordHashVersion = 2,
                SecurityStamp = Guid.NewGuid().ToString(),
                PhoneNumber = input.PhoneNumber,
                LockoutEnabled = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            _context.Users.Add(user);

            // Assign roles
            if (input.Roles != null && input.Roles.Any())
            {
                foreach (var roleName in input.Roles)
                {
                    var role = await _context.Roles
                        .FirstOrDefaultAsync(r => r.Name == roleName);
                    if (role != null)
                    {
                        _context.UserRoles.Add(new UserRole
                        {
                            UserId = user.Id,
                            RoleId = role.Id,
                            AssignedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Result.Success("使用者建立成功", new { user.Id, user.UserName });
        }

        public async Task<Result> UpdateUserAsync(Guid id, UpdateUserInput input)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
            if (user == null)
            {
                return Result.Failure(new[] { "找不到指定的使用者" });
            }

            if (input.Email != null)
            {
                var normalizedEmail = input.Email.ToUpperInvariant();
                if (await _context.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail && u.Id != id))
                {
                    return Result.Failure(new[] { "Email 已被使用" });
                }
                user.Email = input.Email;
                user.NormalizedEmail = normalizedEmail;
            }

            user.PhoneNumber = input.PhoneNumber;
            user.EmailConfirmed = input.EmailConfirmed;
            user.UpdatedAt = DateTime.UtcNow;

            // Update roles
            if (input.Roles != null)
            {
                var existingRoles = await _context.UserRoles
                    .Where(ur => ur.UserId == id)
                    .ToListAsync();
                _context.UserRoles.RemoveRange(existingRoles);

                foreach (var roleName in input.Roles)
                {
                    var role = await _context.Roles
                        .FirstOrDefaultAsync(r => r.Name == roleName);
                    if (role != null)
                    {
                        _context.UserRoles.Add(new UserRole
                        {
                            UserId = id,
                            RoleId = role.Id,
                            AssignedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Result.Success("使用者更新成功", new { user.Id, user.UserName });
        }

        public async Task<Result> DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
            if (user == null)
            {
                return Result.Failure(new[] { "找不到指定的使用者" });
            }

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Result.Success("使用者刪除成功");
        }

        public async Task<Result> ResetPasswordAsync(Guid id, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
            if (user == null)
            {
                return Result.Failure(new[] { "找不到指定的使用者" });
            }

            user.PasswordHash = _authService.HashPassword(newPassword);
            user.PasswordHashVersion = 2;
            user.SecurityStamp = Guid.NewGuid().ToString();
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Result.Success("密碼重設成功");
        }

        public async Task<Result> LockUserAsync(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
            if (user == null)
            {
                return Result.Failure(new[] { "找不到指定的使用者" });
            }

            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Result.Success("使用者已鎖定");
        }

        public async Task<Result> UnlockUserAsync(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
            if (user == null)
            {
                return Result.Failure(new[] { "找不到指定的使用者" });
            }

            user.LockoutEnd = null;
            user.AccessFailedCount = 0;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Result.Success("使用者已解鎖");
        }

        public async Task<IList<string>> GetRolesAsync(Guid userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.Name)
                .ToListAsync();
        }

        public async Task<Result> SetRolesAsync(Guid userId, IList<string> roleNames)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
            if (user == null)
            {
                return Result.Failure(new[] { "找不到指定的使用者" });
            }

            // Remove existing roles
            var existingRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();
            _context.UserRoles.RemoveRange(existingRoles);

            // Add new roles
            foreach (var roleName in roleNames)
            {
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.NormalizedName == roleName.ToUpperInvariant());
                if (role != null)
                {
                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = userId,
                        RoleId = role.Id,
                        AssignedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Result.Success("使用者角色設定成功");
        }
    }
}
