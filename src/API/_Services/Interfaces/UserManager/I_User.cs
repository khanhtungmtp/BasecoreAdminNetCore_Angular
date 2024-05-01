using API.Helpers.Base;
using API.Helpers.Utilities;
using ViewModels.Forum;
using ViewModels.System;
using ViewModels.UserManager;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.UserManager;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_User
{
    // GetMenuByUserPermission
    Task<OperationResult<List<FunctionTreeVM>>> GetMenuByUserPermission(string userId);
    Task<OperationResult<UserVM>> GetByIdAsync(string id);
    Task<OperationResult<PagingResult<UserVM>>> GetPaging(PaginationParam pagination, UserSearchRequest userSearchRequest);
    Task<OperationResult<PagingResult<ForumQuickVM>>> GetForumByUserId(string userId, PaginationParam pagination);
    Task<OperationResult> DeleteRangeAsync(List<string> ids, string idLogedIn);
}
