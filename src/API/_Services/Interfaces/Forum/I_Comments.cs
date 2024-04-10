using API.Helpers.Base;
using API.Helpers.Utilities;
using ViewModels.Forum;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.Forum;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Comments
{
    Task<OperationResult<CommentVM>> FindByIdAsync(int commentId);
    Task<OperationResult<CommentResponseVM>> CreateAsync(int forumId, CommentCreateRequest request);
    Task<OperationResult<List<CommentVM>>> GetRecentCommentsAsync(int take);
    Task<OperationResult<IEnumerable<CommentVM>>> GetCommentTreeByForumIdAsync(int forumId);
    Task<OperationResult<PagingResult<CommentVM>>> GetPagingAsync(string? filter, PaginationParam pagination, CommentVM commentVM);
    Task<OperationResult> PutAsync(int commentId, CommentCreateRequest request);
    Task<OperationResult> DeleteAsync(int forumId, int commentId);
}
