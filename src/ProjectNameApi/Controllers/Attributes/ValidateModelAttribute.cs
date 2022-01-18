using ProjectNameApi.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProjectNameApi.Controllers.Attributes
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.GetValidationErrors();
                var errorStrings = "";

                foreach (var error in errors)
                {
                    errorStrings += error.Errors.JoinToString(", ");
                }
                
                context.Result = new BadRequestObjectResult(new {Message = errorStrings});
            }
        }
    }
}