using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Helpers.Base;

public class OperationResult : ApiResponseBase
{
    private OperationResult(int statusCode, bool success, string message)
        : base(statusCode, success, message) { }

    public static OperationResult Success(string message = "") =>
        new((int)HttpStatusCode.OK, true, message);

    public static OperationResult NotFound(string message = "") =>
        new((int)HttpStatusCode.NotFound, false, message);

    public static OperationResult Conflict(string message = "") =>
        new((int)HttpStatusCode.Conflict, false, message);

    public static OperationResult BadRequest(string message = "") =>
        new((int)HttpStatusCode.BadRequest, false, message);

    // Method for handling invalid ModelState
    public static OperationResult BadRequest(ModelStateDictionary modelState)
    {
        if (modelState.IsValid)
        {
            throw new ArgumentException("ModelState must be invalid", nameof(modelState));
        }

        return new OperationResult((int)HttpStatusCode.BadRequest, false,
            string.Join(", ", modelState.SelectMany(x => x.Value?.Errors?.ToList() ?? [])
    .Select(x => x.ErrorMessage).ToArray()));
    }

    // Method for handling IdentityResult failures
    public static OperationResult BadRequest(IdentityResult identityResult)
    {
        return new OperationResult((int)HttpStatusCode.BadRequest, false,
            string.Join(", ", identityResult.Errors
            .Select(x => x.Code + " - " + x.Description).ToArray()));
    }

}

public class OperationResult<T> : ApiResponseBase<T>
{
    private OperationResult(int statusCode, bool success, string message, T data)
        : base(statusCode, success, message, data) { }

    public static OperationResult<T> Success(T data, string message = "") =>
        new((int)HttpStatusCode.OK, true, message, data);

    public static OperationResult<T> NotFound(string message = "") =>
        new((int)HttpStatusCode.NotFound, false, message, default!);

    public static OperationResult<T> Conflict(string message = "") =>
        new((int)HttpStatusCode.Conflict, false, message, default!);

    public static OperationResult<T> BadRequest(string message = "") =>
        new((int)HttpStatusCode.BadRequest, false, message, default!);
}
