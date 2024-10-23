using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApplicationService.Filters
{
    public class RejectInvalidPpcReportAttribute : ActionFilterAttribute
	{
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = new HttpValidationProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Status = 400,
            };

            if (result.Errors.Count == 0)
            {
                return next();
            }

            context.Result = new BadRequestObjectResult(result);
            return base.OnActionExecutionAsync(context, next);
        }
    }
}
