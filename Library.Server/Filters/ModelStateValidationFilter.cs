using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Zheng.Mdl.Models;

namespace Library.Server.Filters
{
    public class ModelStateValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                context.Result = new BadRequestObjectResult(Result.Failure(errors));
            }
        }
    }
}
