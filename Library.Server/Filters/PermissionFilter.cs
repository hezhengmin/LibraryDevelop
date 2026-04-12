using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Library.Server.Filters
{
    public class PermissionFilter : Attribute, IAsyncAuthorizationFilter
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly PermissionRequirement _requirement;

        public PermissionFilter(IAuthorizationService authorizationService, PermissionRequirement requirement)
        {
            _authorizationService = authorizationService;
            _requirement = requirement;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var result = await _authorizationService.AuthorizeAsync(context.HttpContext.User, null, _requirement);

            if (!result.Succeeded)
            {
                context.Result = new JsonResult("您沒有此操作權限")
                {
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
            }
        }
    }
}
