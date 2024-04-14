using API.Helpers.Base;
using API.Helpers.Utilities;
using API.Models;
using ViewModels.Forum;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.UserManager;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_User
{
    // GetMenuByUserPermission
    Task<OperationResult<List<FunctionVM>>> GetMenuByUserPermission(string userId);
    Task<OperationResult<User>> GetByIdAsync(string id);
    Task<OperationResult<PagingResult<ForumQuickVM>>> GetForumByUserId(string userId, PaginationParam pagination);
}
