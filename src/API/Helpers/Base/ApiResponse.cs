using Newtonsoft.Json;

namespace API.Helpers.Base
{
    public class ApiResponse<T> where T : class
    {
        // Summary:
        // follows errors.
        // Value:
        // Guiid
        public string TrackId { get; set; } = Guid.NewGuid().ToString();
        // Summary:
        // the HTTP status code.
        // Value:
        // number   
        public int Status { get; set; }
        // Summary:
        // flag successful.
        // Value:
        // true | false
        public bool Succeeded { get; private set; }
        // Summary:
        // the message success or error response.
        // Value:
        // string

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; } = string.Empty; // for production
        // Summary:
        // the detail success or error response.
        // Remarks:
        // optional

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Detail { get; set; } // for develop | hide for production
        // Summary:
        // the errors response.
        // Remarks:
        // ! use in custom BadRequestObjectResult

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string>? Errors { get; set; }
        // Summary:
        // the data response.
        // Remarks:
        // optional

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T? Data { get; private set; }

        //
        //  Created Date: 2024-04-06 09:44:54
        // Default constructor 
        //
        public ApiResponse()
        {
        }
        //
        // Summary:
        // ! For Production :
        // Constructor for responses error system 
        // for ADD, Update, Delete,...
        // Parameters:
        //* @ statusCode:     the HTTP status code
        //* @ detail:         the detail error
        // 
        public ApiResponse(int statusCode, string detail)
        {
            Status = statusCode;
            Message = GetDefaultMessageForStatusCode(statusCode);
            Detail = detail ?? GetDefaultMessageForStatusCode(statusCode);
        }

        // 
        // Summary:
        // ! For Developer :
        // Constructor for responses error system
        //
        // Parameters:
        //* @ statusCode:     the HTTP status code
        //* @ message:        the message error
        //* @ detail:         the detail error
        public ApiResponse(int statusCode, string message, string detail)
        {
            Status = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
            Detail = detail;
        }
        //
        // Summary:
        // ! Use in custom BadRequestObjectResult (program.cs)
        // Constructor for responses with errors
        //
        public ApiResponse(int statusCode, string message, IEnumerable<string> errors)
        {
            Status = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
            Errors = errors;
        }
        //
        // Summary:
        // Constructor for responses with data 
        // Remarks:
        // ! Data, No message
        //
        public ApiResponse(int statusCode, bool succeeded, T data)
        {
            Status = statusCode;
            Succeeded = succeeded;
            Data = data;
        }
        //
        // Summary:
        // Constructor for responses with data and message
        // Remarks:
        // ! data and message
        //
        public ApiResponse(int statusCode, bool succeeded, string message, T data)
        {
            Status = statusCode;
            Succeeded = succeeded;
            Message = message;
            Data = data;
        }
        //
        // Summary:
        // helpers get default message for status code
        //
        // Parameters:
        // @ status code
        //
        // Returns:
        // string message error
        //
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