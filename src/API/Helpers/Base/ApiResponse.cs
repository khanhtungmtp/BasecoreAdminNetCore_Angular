using Newtonsoft.Json;

namespace API.Helpers.Base
{
    public class ApiResponse<T> where T : class
    {
        public int StatusCode { get; private set; }
        public bool Succeeded { get; private set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Error { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; private set; } = string.Empty;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T? Data { get; private set; }

        // Constructor for responses without data
        public ApiResponse(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        // Constructor for responses error system without data
        public ApiResponse(int statusCode, string message, string error)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
            Error = error;
        }

        // Constructor for responses with data
        public ApiResponse(bool succeeded, string message, T data)
        {
            Succeeded = succeeded;
            Message = message;
            Data = data;
        }

        // Constructor for responses with data
        public ApiResponse(int statusCode, string message, T data)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
            Data = data;
        }

        private static string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                404 => "Resource not found",
                500 => "An unhandled error occurred",
                _ => "Unknown error"
            };
        }
    }

    // Non-generic version of ApiResponse for convenience when no data is needed
    public class ApiResponse : ApiResponse<object>
    {
        public ApiResponse(int statusCode, string message) : base(statusCode, message)
        {
        }

        // with single error
        public ApiResponse(int statusCode, string message, string error) : base(statusCode, message, error)
        {
        }

        // with list error
        public ApiResponse(int statusCode, string message, IEnumerable<string> errors) : base(statusCode, message, errors)
        {
        }
    }
}