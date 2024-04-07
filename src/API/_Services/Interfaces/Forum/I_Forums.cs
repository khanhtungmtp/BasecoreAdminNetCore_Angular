using API.Helpers.Base;
using API.Helpers.Utilities;
using ViewModels.Forum;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.Forum;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Forums
{
    Task<ApiResponse<string>> CreateAsync(ForumCreateRequest request);
    Task<ApiResponse<PagingResult<ForumQuickVM>>> GetForumsPagingAsync(string? filter, PaginationParam pagination, ForumQuickVM forumVM);
    Task<ApiResponse<List<ForumQuickVM>>> GetLatestForumAsync(int take);
    Task<ApiResponse<List<ForumQuickVM>>> GetPopularForumAsync(int take);
    Task<ApiResponse<PagingResult<ForumQuickVM>>> GetForumByTagIdAsync(string labelId, PaginationParam pagination);
    Task<ApiResponse<ForumVM>> FindByIdAsync(int id);
    Task<ApiResponse> PutAsync(int id, ForumCreateRequest request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse> UpdateViewCountAsync(int id);
    Task<ApiResponse<List<LabelVM>>> GetLabelsByForumIdAsync(int forumId);
}
