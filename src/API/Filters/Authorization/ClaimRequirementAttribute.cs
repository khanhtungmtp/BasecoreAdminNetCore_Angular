using System.Net;
using API.Data;
using API.Helpers.Base;
using API.Helpers.Constants;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;

namespace API.Filters.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ClaimRequirementAttribute(FunctionCode functionCode, CommandCode commandCode) : Attribute, IAuthorizationFilter
{
    private readonly FunctionCode _functionCode = functionCode;
    private readonly CommandCode _commandCode = commandCode;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // skip authorization if action is decorated with [AllowAnonymous] attribute
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        // authorization
        User? user = (User?)context.HttpContext.Items["User"];
        ArgumentNullException.ThrowIfNull(user);

        // get roles where id 
        DataContext? _db = context.HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;
        ArgumentNullException.ThrowIfNull(_db);
        List<string>? roles = [.. _db.UserRoles.Where(x => x.UserId == user.Id).Select(x=>x.RoleId)];

        // get permission where roleID of user
        List<Permission>? permissions = [.. _db.Permissions.Where(x => roles.Contains(x.RoleId))];

        // Check if user roles intersect with the given _roles, if _roles is not empty
        if (permissions is not null)
        {
            var httpContext = context.HttpContext;
            string? trackId = Guid.NewGuid().ToString();
            if (permissions is null || !permissions.Any(x=>x.FunctionId == _functionCode.ToString() && x.CommandId == _commandCode.ToString()))
            {
                context.Result = new UnauthorizedObjectResult(new ErrorGlobalResponse
                {
                    TrackId = trackId,
                    Title = ReasonPhrases.GetReasonPhrase((int)HttpStatusCode.Unauthorized),
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "Unauthorized, Access denied",
                    Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                });
            }
        }
        
    }
}