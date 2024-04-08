using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using NLog;

namespace API.Helpers.Base;

public class GlobalExceptionHandler(IHostEnvironment env) : IExceptionHandler
{
    private readonly IHostEnvironment _env = env;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception ex, CancellationToken cancellationToken)
    {
        Logger logger = LogManager.GetLogger("applog");
        ErrorGlobalResponse? result;
        string? detail = _env.IsDevelopment() ? ex.StackTrace?.ToString() : "Oops, an error occurred."; // show only development

        result = new ErrorGlobalResponse
        {
            TrackId = Guid.NewGuid().ToString(),
            Status = ex switch
            {
                ArgumentNullException or NullReferenceException or KeyNotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError
            },
            Type = ex.GetType().Name,
            Title = ex.Message,
            Detail = detail,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
        };

        // Logging the exception
        logger.Log(NLog.LogLevel.Error, $"{DateTime.UtcNow} - Path: {httpContext?.Request?.Path}  ==> Error: {detail ?? "Details suppressed"}{Environment.NewLine}==> Inner: {ex?.InnerException}");

        // Write the response
        if (httpContext is not null)
        {
            httpContext.Response.StatusCode = result.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(result, cancellationToken: cancellationToken);
            return true;
        }
        return false;
    }
}
