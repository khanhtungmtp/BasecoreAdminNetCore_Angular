namespace API.Helpers.Base;

public class ApiNotFoundResponse(string message) : ApiResponse(404, message)
{
}