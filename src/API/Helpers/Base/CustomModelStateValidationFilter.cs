using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers.Base;

public class CustomModelStateValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var apiResponse = new ApiBadRequestResponse(context.ModelState);
            // context.Result = new ObjectResult(apiResponse) { StatusCode = apiResponse.Status };
        }
    }
}
