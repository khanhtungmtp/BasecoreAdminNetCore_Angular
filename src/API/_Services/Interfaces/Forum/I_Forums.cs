using API.Helpers.Base;
using API.Helpers.Utilities;
using ViewModels.Forum;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.Forum;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Forums
{
    Task<ApiResponse<string>> PostForumAsync(ForumCreateRequest request);
    Task<ApiResponse<PagingResult<ForumQuickVM>>> GetForumsPagingAsync(string? filter, PaginationParam pagination, ForumQuickVM userVM);
}
