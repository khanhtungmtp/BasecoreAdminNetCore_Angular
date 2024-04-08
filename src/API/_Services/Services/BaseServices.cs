using API._Repositories;
using API.Helpers.Base;

namespace API._Services.Services
{
    public class BaseServices(IRepositoryAccessor repoStore)
    {
        protected readonly IRepositoryAccessor _repoStore = repoStore;

        //
        // *Created Date: 2024-04-08 11:49:52
        // Summary:
        // version generic
        protected ApiResponse<T> Success<T>(int statusCode, T data, string message = "")
        {
            return new(statusCode, true, message, data);
        }

        protected ApiResponse<T> Fail<T>(int statusCode, string message = "")
        {
            return new(statusCode, false, message, default);
        }

        //
        // *Created Date: 2024-04-08 11:49:52
        // Summary:
        // version non generic
        protected ApiResponse Success(int statusCode, string message = "")
        {
            return new ApiResponse(statusCode, true, message);
        }

        protected ApiResponse Fail(int statusCode, string message = "")
        {
            return new ApiResponse(statusCode, false, message);
        }

    }
}