using API._Repositories;
using API._Services.Interfaces.Forum;
using API.Helpers.Base;
using API.Helpers.Constants;
using API.Helpers.Utilities;
using API.Models;
using Microsoft.EntityFrameworkCore;
using ViewModels.Forum;

namespace API._Services.Services.Forum;
public class S_Comments(IRepositoryAccessor repoStore) : BaseServices(repoStore), I_Comments
{
    public async Task<OperationResult<CommentResponseVM>> CreateAsync(int forumId, CommentCreateRequest request)
    {
        var comment = new Comment()
        {
            Content = request.Content,
            ForumId = forumId,
            OwnwerUserId = request.UserId,
            ReplyId = request.ReplyId
        };
        _repoStore.Comments.Add(comment);

        var forum = await _repoStore.Forums.FindAsync(forumId);
        if (forum is null)
            return OperationResult<CommentResponseVM>.NotFound($"Cannot found forum with id: {forumId}");

        forum.NumberOfComments = forum.NumberOfComments.GetValueOrDefault(0) + 1;
        _repoStore.Forums.Update(forum);

        bool result = await _repoStore.SaveChangesAsync();
        if (result)
        {
            //Send mail
            if (comment.ReplyId.HasValue)
            {
                Comment? repliedComment = await _repoStore.Comments.FindAsync(comment.ReplyId.Value);
                User? repledUser = await _repoStore.Users.FindAsync(repliedComment.OwnwerUserId);
                var emailModel = new RepliedCommentVM()
                {
                    CommentContent = request.Content,
                    ForumId = forumId,
                    ForumSeoAlias = forum.SeoAlias,
                    ForumTitle = forum.Title,
                    RepliedName = repledUser.FullName
                };
                //https://github.com/leemunroe/responsive-html-email-template
                // var htmlContent = await _viewRenderService.RenderToStringAsync("_RepliedCommentEmail", emailModel);
                // await _emailSender.SendEmailAsync(repledUser.Email, "Có người đang trả lời bạn", htmlContent);
            }
            return OperationResult<CommentResponseVM>.Success(new CommentResponseVM { ForumId = forumId.ToString() }, "Create comment successfully.");
        }
        else
            return OperationResult<CommentResponseVM>.BadRequest("Create comment failed");

    }

    public async Task<OperationResult<CommentVM>> FindByIdAsync(int commentId)
    {
        var comment = await _repoStore.Comments.FindAsync(commentId);
        if (comment is null)
            return OperationResult<CommentVM>.NotFound($"Cannot found comment with id: {commentId}");
        var user = await _repoStore.Users.FindAsync(comment.OwnwerUserId);
        if (user is null)
            return OperationResult<CommentVM>.NotFound($"Cannot found user with id: {comment.OwnwerUserId}");
        var commentVm = new CommentVM()
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedDate = comment.CreatedDate,
            ForumId = comment.ForumId,
            UpdatedDate = comment.UpdatedDate,
            OwnerUserId = comment.OwnwerUserId,
            OwnerName = user.FullName
        };

        return OperationResult<CommentVM>.Success(commentVm, "Get comment successfully.");
    }

    public async Task<OperationResult<PagingResult<CommentVM>>> GetPagingAsync(string? filter, PaginationParam pagination, CommentVM commentVM)
    {
        var query = from c in _repoStore.Comments.FindAll(true)
                    join u in _repoStore.Users.FindAll(true)
                        on c.OwnwerUserId equals u.Id
                    select new { c, u };
        if (commentVM.ForumId.HasValue)
        {
            query = query.Where(x => x.c.ForumId == commentVM.ForumId.Value);
        }
        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(x => x.c.Content.Contains(filter));
        }
        var result = await query.Select(c => new CommentVM()
        {
            Id = c.c.Id,
            Content = c.c.Content,
            CreatedDate = c.c.CreatedDate,
            ForumId = c.c.ForumId,
            UpdatedDate = c.c.UpdatedDate,
            OwnerUserId = c.c.OwnwerUserId,
            OwnerName = c.u.FullName
        }).ToListAsync();
        var resultsPaging = PagingResult<CommentVM>.Create(result, pagination.PageNumber, pagination.PageSize);
        return OperationResult<PagingResult<CommentVM>>.Success(resultsPaging, "Get forums successfully.");

    }

    public async Task<OperationResult> PutAsync(int commentId, CommentCreateRequest request)
    {
        var comment = await _repoStore.Comments.FindAsync(commentId);
        if (comment is null)
            return OperationResult.NotFound($"Cannot found comment with id: {commentId}");
        if (comment.OwnwerUserId != request.UserId)
            return OperationResult.NotFound($"Cannot found user with id: {commentId}");

        comment.Content = request.Content;
        _repoStore.Comments.Update(comment);

        bool result = await _repoStore.SaveChangesAsync();

        if (result)
            return OperationResult.Success("Update comment successfully");
        return OperationResult.BadRequest("Update comment failed");
    }

    public async Task<OperationResult> DeleteAsync(int forumId, int commentId)
    {
        var comment = await _repoStore.Comments.FindAsync(commentId);
        if (comment is null)
            return OperationResult.NotFound($"Cannot found comment with id: {commentId}");

        _repoStore.Comments.Remove(comment);

        var forum = await _repoStore.Forums.FindAsync(forumId);
        if (forum is null)
            return OperationResult.NotFound($"Cannot found forum with id: {commentId}");

        forum.NumberOfComments = forum.NumberOfComments.GetValueOrDefault(0) - 1;
        _repoStore.Forums.Update(forum);

        bool result = await _repoStore.SaveChangesAsync();
        if (result)
            return OperationResult.Success("Delete comment successfully");
        return OperationResult.BadRequest("Delete comment failed");
    }

    public async Task<OperationResult<List<CommentVM>>> GetRecentCommentsAsync(int take)
    {
            var query = from c in _repoStore.Comments.FindAll(true)
                        join u in _repoStore.Users.FindAll(true)
                            on c.OwnwerUserId equals u.Id
                        join k in _repoStore.Forums.FindAll(true)
                        on c.ForumId equals k.Id
                        orderby c.CreatedDate descending
                        select new { c, u, k };

        List<CommentVM>? comments = await query.Take(take).Select(x => new CommentVM()
            {
                Id = x.c.Id,
                CreatedDate = x.c.CreatedDate,
                ForumId = x.c.ForumId,
                OwnerUserId = x.c.OwnwerUserId,
                ForumTitle = x.k.Title,
                OwnerName = x.u.FullName,
                ForumSeoAlias = x.k.SeoAlias
            }).ToListAsync();

        return OperationResult<List<CommentVM>>.Success(comments, "Get recent comments successfully.");
    }

    public async Task<OperationResult<IEnumerable<CommentVM>>> GetCommentTreeByForumIdAsync(int forumId)
    {
        var query = from c in _repoStore.Comments.FindAll(true)
                    join u in _repoStore.Users.FindAll(true)
                        on c.OwnwerUserId equals u.Id
                    where c.ForumId == forumId
                    select new { c, u };

        var flatComments = await query.Select(x => new CommentVM()
        {
            Id = x.c.Id,
            Content = x.c.Content,
            CreatedDate = x.c.CreatedDate,
            ForumId = x.c.ForumId,
            OwnerUserId = x.c.OwnwerUserId,
            OwnerName = x.u.FullName,
            ReplyId = x.c.ReplyId
        }).ToListAsync();

        var lookup = flatComments.ToLookup(c => c.ReplyId);
        var rootCategories = flatComments.Where(x => x.ReplyId == null);

        foreach (var c in rootCategories)//only loop through root categories
        {
            // you can skip the check if you want an empty list instead of null
            // when there is no children
            if (lookup.Contains(c.Id))
                c.Children = lookup[c.Id].ToList();
        }

        return OperationResult<IEnumerable<CommentVM>>.Success(rootCategories, "Get comment tree successfully.");
    }
}
