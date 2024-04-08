using System.Net;
using API._Repositories;
using API._Services.Interfaces.Forum;
using API._Services.Interfaces.System;
using API.Helpers.Base;
using API.Helpers.Constants;
using API.Helpers.Utilities;
using API.Models;
using Microsoft.EntityFrameworkCore;
using ViewModels.Forum;

namespace API._Services.Services.Forum;
public class S_Comments(IRepositoryAccessor repoStore, I_Cache cacheService) : BaseServices(repoStore), I_Comments
{
    private readonly I_Cache _cacheService = cacheService;

    public async Task<ApiResponse<CommentResponseVM>> CreateAsync(int forumId, CommentCreateRequest request)
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
        if (forum == null)
            return new ApiResponse<CommentResponseVM>((int)HttpStatusCode.NotFound, false, $"Cannot found forum with id: {forumId}", null!);

        forum.NumberOfComments = forum.NumberOfComments.GetValueOrDefault(0) + 1;
        _repoStore.Forums.Update(forum);

        bool result = await _repoStore.SaveChangesAsync();
        if (result)
        {
            await _cacheService.RemoveAsync(CacheConstants.RecentComments);

            //Send mail
            if (comment.ReplyId.HasValue)
            {
                var repliedComment = await _repoStore.Comments.FindAsync(comment.ReplyId.Value);
                var repledUser = await _repoStore.Users.FindAsync(repliedComment.OwnwerUserId);
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
            return new ApiResponse<CommentResponseVM>((int)HttpStatusCode.OK, true, "Create comment successfully.", new CommentResponseVM { ForumId = forumId.ToString() });
        }
        else
            return new ApiResponse<CommentResponseVM>((int)HttpStatusCode.BadRequest, false, "Create comment failed", null!);

    }

    public async Task<ApiResponse<CommentVM>> FindByIdAsync(int commentId)
    {
        var comment = await _repoStore.Comments.FindAsync(commentId);
        if (comment is null)
            return new ApiResponse<CommentVM>((int)HttpStatusCode.NotFound, false, $"Cannot found comment with id: {commentId}", null!);
        var user = await _repoStore.Users.FindAsync(comment.OwnwerUserId);
        if (user is null)
            return new ApiResponse<CommentVM>((int)HttpStatusCode.NotFound, false, $"Cannot found user with id: {comment.OwnwerUserId}", null!);
        var commentVm = new CommentVM()
        {
            Id = comment.Id,
            Content = comment.Content,
            CreateDate = comment.CreateDate,
            ForumId = comment.ForumId,
            UpdateDate = comment.UpdateDate,
            OwnerUserId = comment.OwnwerUserId,
            OwnerName = user.FullName
        };

        return new ApiResponse<CommentVM>((int)HttpStatusCode.OK, true, "Get comment successfully.", commentVm);
    }

    public async Task<ApiResponse<PagingResult<CommentVM>>> GetCommentsPagingAsync(string? filter, PaginationParam pagination, CommentVM commentVM)
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
            CreateDate = c.c.CreateDate,
            ForumId = c.c.ForumId,
            UpdateDate = c.c.UpdateDate,
            OwnerUserId = c.c.OwnwerUserId,
            OwnerName = c.u.FullName
        }).ToListAsync();
        var resultsPaging = PagingResult<CommentVM>.Create(result, pagination.PageNumber, pagination.PageSize);
        return new ApiResponse<PagingResult<CommentVM>>((int)HttpStatusCode.OK, true, "Get forums successfully.", resultsPaging);

    }

    public async Task<ApiResponse> PutAsync(int commentId, CommentCreateRequest request)
    {
        var comment = await _repoStore.Comments.FindAsync(commentId);
        if (comment is null)
            return new ApiNotFoundResponse($"Cannot found comment with id: {commentId}");
        if (comment.OwnwerUserId != request.UserId)
            return new ApiNotFoundResponse($"Cannot found user with id: {commentId}");

        comment.Content = request.Content;
        _repoStore.Comments.Update(comment);

        bool result = await _repoStore.SaveChangesAsync();

        if (result)
            return new ApiResponse((int)HttpStatusCode.OK, true, "Update comment successfully");
        return new ApiResponse((int)HttpStatusCode.BadRequest, false, "Update comment failed");
    }

    public async Task<ApiResponse> DeleteAsync(int forumId, int commentId)
    {
        var comment = await _repoStore.Comments.FindAsync(commentId);
        if (comment is null)
            return new ApiNotFoundResponse($"Cannot found comment with id: {commentId}");

        _repoStore.Comments.Remove(comment);

        var forum = await _repoStore.Forums.FindAsync(forumId);
        if (forum is null)
            return new ApiNotFoundResponse($"Cannot found forum with id: {commentId}");

        forum.NumberOfComments = forum.NumberOfComments.GetValueOrDefault(0) - 1;
        _repoStore.Forums.Update(forum);

        bool result = await _repoStore.SaveChangesAsync();
        if (result)
        {
            //Delete cache
            await _cacheService.RemoveAsync(CacheConstants.RecentComments);
            return new ApiResponse((int)HttpStatusCode.OK, true, "Delete comment successfully");
        }
        return new ApiResponse((int)HttpStatusCode.OK, false, "Delete comment failed");
    }

    public async Task<ApiResponse<List<CommentVM>>> GetRecentCommentsAsync(int take)
    {
        var cachedData = await _cacheService.GetAsync<List<CommentVM>>(CacheConstants.RecentComments);
        if (cachedData == null)
        {
            var query = from c in _repoStore.Comments.FindAll(true)
                        join u in _repoStore.Users.FindAll(true)
                            on c.OwnwerUserId equals u.Id
                        join k in _repoStore.Forums.FindAll(true)
                        on c.ForumId equals k.Id
                        orderby c.CreateDate descending
                        select new { c, u, k };

            var comments = await query.Take(take).Select(x => new CommentVM()
            {
                Id = x.c.Id,
                CreateDate = x.c.CreateDate,
                ForumId = x.c.ForumId,
                OwnerUserId = x.c.OwnwerUserId,
                ForumTitle = x.k.Title,
                OwnerName = x.u.FullName,
                ForumSeoAlias = x.k.SeoAlias
            }).ToListAsync();

            await _cacheService.SetAsync(CacheConstants.RecentComments, comments);
            cachedData = comments;
        }

        return new ApiResponse<List<CommentVM>>((int)HttpStatusCode.OK, true, "Get recent comments successfully.", cachedData);
    }

    public async Task<ApiResponse<IEnumerable<CommentVM>>> GetCommentTreeByForumIdAsync(int forumId)
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
            CreateDate = x.c.CreateDate,
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

        return new ApiResponse<IEnumerable<CommentVM>>((int)HttpStatusCode.OK, true, "Get comment tree successfully.", rootCategories);
    }
}
