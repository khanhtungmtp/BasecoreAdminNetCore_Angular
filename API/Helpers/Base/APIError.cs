using System.Text.Json;

namespace API.Helpers.Base;

public class APIError
{
    public APIError() { }

    public APIError(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
    }

    public APIError(int errorCode, string errorMessage, string errorDetails)
    {
        ErrorCode = errorCode;
        this.ErrorMessage = errorMessage;
        ErrorDetails = errorDetails;
    }

    public int ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorDetails { get; set; }
    public int StatusCode { get; }
    public string? Message { get; }

    public override string ToString()
    {
        var options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return JsonSerializer.Serialize(this, options);
    }
}
