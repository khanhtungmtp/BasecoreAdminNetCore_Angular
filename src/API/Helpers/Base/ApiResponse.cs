using Newtonsoft.Json;

namespace API.Helpers.Base
{
    public class ApiResponse<T> where T : class
    {
        public string TrackId { get; set; } = Guid.NewGuid().ToString();
        public int Status { get; set; }
        public bool Succeeded { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; } = string.Empty; // for production

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Detail { get; set; } // for develop | hide for production

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string>? Errors { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T? Data { get; private set; }

        public ApiResponse()
        {
        }

        // Constructor for responses without data (not found, response error, etc)
        public ApiResponse(int statusCode, string detail)
        {
            Status = statusCode;
            Message = GetDefaultMessageForStatusCode(statusCode);
            Detail = detail ?? GetDefaultMessageForStatusCode(statusCode);
        }

        // Constructor for responses error system without data
        public ApiResponse(int statusCode, string message, string detail)
        {
            Status = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
            Detail = detail;
        }

        // Constructor for responses list error system without data
        public ApiResponse(int statusCode, string message, IEnumerable<string> errors)
        {
            Status = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
            Errors = errors;
        }

        // Constructor for responses with data no message
        public ApiResponse(int statusCode, bool succeeded, T data)
        {
            Status = statusCode;
            Succeeded = succeeded;
            Data = data;
        }

        // Constructor for responses with data include message
        public ApiResponse(int statusCode, bool succeeded, string message, T data)
        {
            Status = statusCode;
            Succeeded = succeeded;
            Message = message;
            Data = data;
        }

        private static string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "One or more validation errors occurred.",
                404 => "Resource not found",
                500 => "An unhandled error occurred",
                _ => "Unknown error"
            };
        }
    }

    // Non-generic version of ApiResponse for convenience when no data is needed
    public class ApiResponse : ApiResponse<object>
    {
        public ApiResponse()
        {
        }

        public ApiResponse(int statusCode, string detail) : base(statusCode, detail)
        {
        }

        // with single error
        public ApiResponse(int statusCode, string detail, string error) : base(statusCode, detail, error)
        {
        }

        // with list error
        public ApiResponse(int statusCode, string detail, IEnumerable<string> errors) : base(statusCode, detail, errors)
        {
        }
    }
}