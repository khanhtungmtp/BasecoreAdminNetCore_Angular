using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Helpers.Base;

public class ApiBadRequestResponse : ApiResponse
{
    public new IEnumerable<string> Errors { get; }

    public ApiBadRequestResponse(ModelStateDictionary modelState)
        : base(400, "Bad Request due to invalid model state.")
    {
        if (modelState.IsValid)
        {
            throw new ArgumentException("ModelState must be invalid", nameof(modelState));
        }

        Errors = modelState.SelectMany(x => x.Value?.Errors ?? [])
            .Select(x => x.ErrorMessage).ToArray();
    }

    public ApiBadRequestResponse(IdentityResult identityResult)
       : base(400, "Bad Request due to identity result error.")
    {
        Errors = identityResult.Errors
            .Select(x => x.Code + " - " + x.Description).ToArray();
    }

    public ApiBadRequestResponse(string message)
       : base(400, message)
    {
        Errors = []; // Initialize Errors with an empty array
    }
}