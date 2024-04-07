//  Created Date: 2024-04-07 23:48:54
using System.Text.Json.Serialization;

namespace API.Helpers.Base
{
    public class ApiResponse<T>
    {
        // Summary:
        // follows errors.
        // Value:
        // Guiid
        [JsonPropertyName("trackId")]
        public string TrackId { get; set; } = Guid.NewGuid().ToString();

        // Summary:
        // the HTTP status code.
        // Value:
        // number   
        [JsonPropertyName("status")]
        public int Status { get; set; }

        // Summary:
        // flag successful.
        // Value:
        // true | false
        [JsonPropertyName("succeeded")]
        public bool Succeeded { get; private set; }

        // Summary:
        // the message success or error response.
        // Value:
        // string
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty; // title set where status code

        // Summary:
        // the message success or error response.
        // Remarks:
        // optional
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("message")]
        public string? Message { get; set; } // message success or error for user

        // Summary:
        // the data response.
        // Remarks:
        // optional
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("data")]
        public T? Data { get; private set; }

        //
        // Default constructor 
        //
        public ApiResponse()
        {
        }

        //
        // Summary:
        // Constructor for responses error system 
        //  for ADD, Update, Delete,...
        // Parameters:
        //* @ statusCode:     the HTTP status code
        //* @ message:         the message error
        // 
        public ApiResponse(int statusCode, bool succeeded, string message)
        {
            Status = statusCode;
            Succeeded = succeeded;
            Title = GetDefaultMessageForStatusCode(statusCode);
            Message = message;
        }

        //
        // Summary:
        //  Constructor for responses with data 
        // Remarks:
        // ! Data no inculde message
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
        // ! Data with message
        //
        public ApiResponse(int statusCode, bool succeeded, string message, T data)
        {
            Status = statusCode;
            Succeeded = succeeded;
            Title = GetDefaultMessageForStatusCode(statusCode);
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

        public ApiResponse(int statusCode, bool succeeded, string message) : base(statusCode, succeeded, message)
        {
        }

    }
}