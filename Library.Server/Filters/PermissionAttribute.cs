using Microsoft.AspNetCore.Mvc;

namespace Library.Server.Filters
{
    public class PermissionAttribute : TypeFilterAttribute
    {
        public PermissionAttribute(params string[] codes)
            : base(typeof(PermissionFilter))
        {
            Arguments = new[] { new PermissionRequirement(codes) };
        }
    }
}
