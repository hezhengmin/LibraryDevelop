using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zheng.Bll.Interfaces;
using Zheng.Infra.Data.Models;

namespace Zheng.Bll.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly LibraryContext _context;

        public PermissionService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IList<string>> GetUserPermissionsAsync(Guid userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission)
                .Distinct()
                .ToListAsync();
        }
    }
}
