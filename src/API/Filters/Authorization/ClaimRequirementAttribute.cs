using System.Net;
using System.Security.Claims;
using API.Data;
using API.Helpers.Base;
using API.Helpers.Constants;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;
using ViewModels.UserManager;

namespace API.Filters.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ClaimRequirementAttribute(FunctionCode functionCode, CommandCode commandCode) : Attribute, IAuthorizationFilter
{
    private readonly FunctionCode _functionCode = functionCode;
    private readonly CommandCode _commandCode = commandCode;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var httpContext = context.HttpContext;
        string? trackId = Guid.NewGuid().ToString();
        string? userId = context.HttpContext.User.Claims
               .SingleOrDefault(c => c.Type == SystemConstants.Claims.Id)?.Value;
        // skip authorization if action is decorated with [AllowAnonymous] attribute
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        // authorization
        if (userId is null)
        {
            context.Result = new UnauthorizedObjectResult(new ErrorGlobalResponse
            {
                TrackId = trackId,
                Message = ReasonPhrases.GetReasonPhrase((int)HttpStatusCode.Unauthorized),
                StatusCode = StatusCodes.Status401Unauthorized,
                Detail = "Unauthorized, Access denied",
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            });
            return;
        }

        // get roles where id 
        DataContext? _db = context.HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;
        ArgumentNullException.ThrowIfNull(_db);
        List<string>? roles = [.. _db.UserRoles.Where(x => x.UserId == userId).Select(x => x.RoleId)];

        // get permission where roleID of user
        List<Permission>? permissions = [.. _db.Permissions.Where(x => roles.Contains(x.RoleId))];

        // Check if user roles intersect with the given _roles, if _roles is not empty
        if (permissions is null || !permissions.Any(x=>x.FunctionId == _functionCode.ToString() && x.CommandId == _commandCode.ToString()))
        {
            context.Result = new UnauthorizedObjectResult(new ErrorGlobalResponse
            {
                TrackId = trackId,
                StatusCode = StatusCodes.Status403Forbidden,
                Message = ReasonPhrases.GetReasonPhrase((int)HttpStatusCode.Forbidden),
                Detail = "Access denied",
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            }){StatusCode = StatusCodes.Status403Forbidden};
        }

    }
}