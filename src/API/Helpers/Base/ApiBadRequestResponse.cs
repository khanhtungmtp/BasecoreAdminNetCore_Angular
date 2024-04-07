using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Helpers.Base;

public class ApiBadRequestResponse : ApiResponse
{
    public IEnumerable<string> Errors { get; }

    //* use for model state 
    public ApiBadRequestResponse(ModelStateDictionary modelState)
        : base(400, false, "Bad Request due to invalid model state.")
    {
        if (modelState.IsValid)
        {
            throw new ArgumentException("ModelState must be invalid", nameof(modelState));
        }

        Errors = modelState.SelectMany(x => x.Value?.Errors ?? [])
            .Select(x => x.ErrorMessage).ToArray();
    }

    //* use for identity result 
    public ApiBadRequestResponse(IdentityResult identityResult)
       : base(400, false, "Bad Request due to identity result error.")
    {
        Errors = identityResult.Errors
            .Select(x => x.Code + " - " + x.Description).ToArray();
    }

    //* for user custom message 
    public ApiBadRequestResponse(string message)
       : base(400, false, message)
    {
        Errors = [];
    }
}