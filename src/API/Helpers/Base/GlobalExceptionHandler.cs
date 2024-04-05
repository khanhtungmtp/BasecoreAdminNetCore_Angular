using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using NLog;

namespace API.Helpers.Base;

public class GlobalExceptionHandler(IHostEnvironment env) : IExceptionHandler
{
    private readonly IHostEnvironment _env = env;

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception ex, CancellationToken cancellationToken)
    {
        var (statusCode, message) = HandleException(ex);

        var response = new ApiResponse((int)statusCode, message);

        if (_env.IsDevelopment())
        {
            response = new ApiResponse((int)statusCode, ex.Message, ex.StackTrace ?? "No stack trace available");
        }

        Logger logger = LogManager.GetLogger("applog");
        logger.Log(NLog.LogLevel.Error, $"{DateTime.UtcNow} - Path: {context?.Request?.Path} - Body: {response}{Environment.NewLine} ==> Error: {ex} {Environment.NewLine}==> Inner: {ex?.InnerException}");

        if (context != null)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            // Fixed: Serialize the ApiResponse object to JSON
            await context.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }
        return false;
    }


    private (HttpStatusCode, string) HandleException(Exception ex)
    {
        var exceptionType = ex.GetType();
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "Internal Server Error";

        if (exceptionType == typeof(ApplicationException))
        {
            statusCode = HttpStatusCode.BadRequest;
            message = "Bad Request";
        }
        else if (exceptionType == typeof(KeyNotFoundException) || exceptionType == typeof(NullReferenceException))
        {
            statusCode = HttpStatusCode.NotFound;
            message = "Not Found";
        }
        else if (exceptionType == typeof(UnauthorizedAccessException))
        {
            statusCode = HttpStatusCode.Unauthorized;
            message = "Permission Denied";
        }

        return (statusCode, message);
    }
}
