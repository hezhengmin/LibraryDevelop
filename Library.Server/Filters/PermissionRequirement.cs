using Microsoft.AspNetCore.Authorization;

namespace Library.Server.Filters
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string[] Permissions { get; }

        public PermissionRequirement(params string[] permissions)
        {
            Permissions = permissions;
        }
    }
}
