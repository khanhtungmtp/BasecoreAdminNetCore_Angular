using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Helpers.Base;

public class ApiBadRequestResponse : ApiResponseBase
{

    //* use for model state 
    public ApiBadRequestResponse(ModelStateDictionary modelState)
        : base(400, false, string.Join(", ", modelState.SelectMany(x => x.Value?.Errors ?? [])
            .Select(x => x.ErrorMessage).ToArray()))
    {
        if (modelState.IsValid)
        {
            throw new ArgumentException("ModelState must be invalid", nameof(modelState));
        }

    }

    //* use for identity result 
    public ApiBadRequestResponse(IdentityResult identityResult)
        : base(400, false, string.Join(", ", identityResult.Errors
                .Select(x => x.Code + " - " + x.Description).ToArray()))
    {
    }

    //* for user custom message 
    public ApiBadRequestResponse(string message)
       : base(400, false, message)
    {
    }
}