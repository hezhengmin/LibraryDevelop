using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zheng.Infra.Data.Models;
using Zheng.Mdl.Models;

namespace Zheng.Bll.Interfaces
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public bool IsActive { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class CreateUserInput
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public IList<string>? Roles { get; set; }
    }

    public class UpdateUserInput
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public IList<string>? Roles { get; set; }
    }

    public interface IUserService
    {
        Task<User?> FindByIdAsync(Guid id);
        Task<User?> FindByNameAsync(string userName);
        Task<User?> FindByEmailAsync(string email);
        Task<PagedResult<UserDto>> GetUsersAsync(int page, int pageSize, string? keyword = null);
        Task<UserDto?> GetUserWithRolesAsync(Guid id);
        Task<Result> CreateUserAsync(CreateUserInput input);
        Task<Result> UpdateUserAsync(Guid id, UpdateUserInput input);
        Task<Result> DeleteUserAsync(Guid id);
        Task<Result> ResetPasswordAsync(Guid id, string newPassword);
        Task<Result> LockUserAsync(Guid id);
        Task<Result> UnlockUserAsync(Guid id);
        Task<IList<string>> GetRolesAsync(Guid userId);
        Task<Result> SetRolesAsync(Guid userId, IList<string> roleNames);
    }
}
