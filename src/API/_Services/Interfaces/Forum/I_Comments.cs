using API.Helpers.Base;
using API.Helpers.Utilities;
using ViewModels.Forum;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.Forum;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Comments
{
    Task<ApiResponse<CommentVM>> FindByIdAsync(int commentId);
    Task<ApiResponse<CommentResponseVM>> CreateAsync(int forumId, CommentCreateRequest request);
    Task<ApiResponse<List<CommentVM>>> GetRecentCommentsAsync(int take);
    Task<ApiResponse<IEnumerable<CommentVM>>> GetCommentTreeByForumIdAsync(int forumId);
    Task<ApiResponse<PagingResult<CommentVM>>> GetCommentsPagingAsync(string? filter, PaginationParam pagination, CommentVM commentVM);
    Task<ApiResponse> PutAsync(int commentId, CommentCreateRequest request);
    Task<ApiResponse> DeleteAsync(int forumId, int commentId);
}
