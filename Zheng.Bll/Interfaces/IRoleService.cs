using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zheng.Mdl.Models;

namespace Zheng.Bll.Interfaces
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int UserCount { get; set; }
    }

    public class RoleDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public IList<string> Permissions { get; set; } = new List<string>();
    }

    public class CreateRoleInput
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public IList<string>? Permissions { get; set; }
    }

    public class UpdateRoleInput
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public IList<string>? Permissions { get; set; }
    }

    public interface IRoleService
    {
        Task<PagedResult<RoleDto>> GetRolesAsync(int page, int pageSize);
        Task<RoleDetailDto?> GetRoleWithPermissionsAsync(Guid id);
        Task<Result> CreateRoleAsync(CreateRoleInput input);
        Task<Result> UpdateRoleAsync(Guid id, UpdateRoleInput input);
        Task<Result> DeleteRoleAsync(Guid id);
        Task<IList<string>> GetPermissionsAsync(Guid roleId);
        Task<IList<UserDto>> GetUsersInRoleAsync(Guid roleId);
    }
}
