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
        string? message = _env.IsDevelopment() && ex.GetType().Name == "SqlException" ? ex.Message?.ToString() : "Oops, an error occurred."; // show only development

        result = new ErrorGlobalResponse
        {
            TrackId = Guid.NewGuid().ToString(),
            StatusCode = (int)(HttpStatusCode)httpContext.Response.StatusCode,
            Type = ex.GetType().Name,
            Message = message,
            Detail = detail,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
        };

        // Logging the exception
        logger.Log(NLog.LogLevel.Error, $"{DateTime.UtcNow} - Path: {httpContext?.Request?.Path}  ==> Error: {detail ?? "Details suppressed"}{Environment.NewLine}==> Inner: {ex?.InnerException}");

        // Write the response
        if (httpContext is not null)
        {
            httpContext.Response.StatusCode = result.StatusCode;
            await httpContext.Response.WriteAsJsonAsync(result, cancellationToken: cancellationToken);
            return true;
        }
        return false;
    }
}
