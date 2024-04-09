using API.Helpers.Base;
using API.Helpers.Utilities;
using ViewModels.Forum;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.Forum;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Forums
{
    Task<OperationResult<string>> CreateAsync(ForumCreateRequest request);
    Task<OperationResult<PagingResult<ForumQuickVM>>> GetForumsPagingAsync(string? filter, PaginationParam pagination, ForumQuickVM forumVM);
    Task<OperationResult<List<ForumQuickVM>>> GetLatestForumAsync(int take);
    Task<OperationResult<List<ForumQuickVM>>> GetPopularForumAsync(int take);
    Task<OperationResult<PagingResult<ForumQuickVM>>> GetForumByTagIdAsync(string labelId, PaginationParam pagination);
    Task<OperationResult<ForumVM>> FindByIdAsync(int id);
    Task<OperationResult> PutAsync(int id, ForumCreateRequest request);
    Task<OperationResult<string>> DeleteAsync(int id);
    Task<OperationResult> UpdateViewCountAsync(int id);
    Task<OperationResult<List<LabelVM>>> GetLabelsByForumIdAsync(int forumId);
}
