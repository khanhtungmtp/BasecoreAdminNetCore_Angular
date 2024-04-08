//  Created Date: 2024-04-08 11:00:54
using System.Text.Json.Serialization;

namespace API.Helpers.Base
{
    public class ApiResponse<T>(int statusCode, bool succeeded, string? message = null, T? data = default)
    {
        // Summary:
        // follows errors.
        // Value: Guiid
        [JsonPropertyName("trackId")]
        public string TrackId { get; } = Guid.NewGuid().ToString();

        // Summary:
        // the HTTP status code.
        // Value:
        // number   
        [JsonPropertyName("status")]
        public int Status { get; } = statusCode;

        // Summary:
        // flag successful.
        // Value:
        // true | false
        [JsonPropertyName("succeeded")]
        public bool Succeeded { get; } = succeeded;

        // Summary:
        // the message success or error response.
        // Value:
        // string
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("title")]
        public string Title { get; } = GetDefaultMessageForStatusCode(statusCode);

        // Summary:
        // the message success or error response.
        // Remarks:
        // optional
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("message")]
        public string? Message { get; } = message;

        // Summary:
        // the data response.
        // Remarks:
        // optional
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("data")]
        public T? Data { get; } = data;

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
                200 or 201 => "Successfully",
                400 => "One or more validation errors occurred.",
                404 => "Resource not found",
                500 => "An unhandled error occurred",
                _ => "Unknown error"
            };
        }
    }

    public class ApiResponse(int status, bool succeeded, string message) : ApiResponse<object>(status, succeeded, message, null)
    {
    }
}