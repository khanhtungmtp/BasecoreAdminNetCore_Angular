using API._Services.Interfaces.Forum;
using API.Helpers.Utilities;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Forum;

namespace API.Controllers.Forums;

public class CommentsController(I_Comments commentService, UserManager<User> userManager) : BaseController
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly I_Comments _commentService = commentService;

    [HttpPost("{forumId}/comments")]
    public async Task<IActionResult> PostComment(int forumId, CommentCreateRequest request)
    {
        request.UserId = _userManager.GetUserId(User) ?? string.Empty;
        var result = await _commentService.CreateAsync(forumId, request);
        if (!result.Succeeded)
        {
            if (result.Status == 404)
                return NotFound(result);
            else
                return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("{forumId}/comments/{commentId}")]
    public async Task<IActionResult> GetCommentDetail(int commentId)
    {
        var result = await _commentService.FindByIdAsync(commentId);
        if (!result.Succeeded)
        {
            if (result.Status == 404)
                return NotFound(result);
            else
                return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("comments/recent/{take}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRecentComments(int take)
    {
        var result = await _commentService.GetRecentCommentsAsync(take);
        return Ok(result);
    }

    [HttpGet("{forumId}/comments/tree")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCommentTreeByForumId(int forumId)
    {
        var result = await _commentService.GetCommentTreeByForumIdAsync(forumId);
        return Ok(result);
    }

    [HttpGet("{forumId}/comments/filter")]
    public async Task<IActionResult> GetCommentsPaging(string? filter, PaginationParam pagination, [FromQuery] CommentVM commentVM)
    {
        var result = await _commentService.GetCommentsPagingAsync(filter, pagination, commentVM);
        return Ok(result);
    }

    [HttpPut("{forumId}/comments/{commentId}")]
    public async Task<IActionResult> PutComment(int commentId, CommentCreateRequest request)
    {
        var result = await _commentService.PutAsync(commentId, request);
        if (!result.Succeeded)
        {
            if (result.Status == 404)
                return NotFound(result);
            else
                return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpDelete("{forumId}/comments/{commentId}")]
    public async Task<IActionResult> DeleteComment(int forumId, int commentId)
    {
        var result = await _commentService.DeleteAsync(forumId, commentId);
        if (!result.Succeeded)
        {
            if (result.Status == 404)
                return NotFound(result);
            else
                return BadRequest(result);
        }
        return Ok(result);
    }


}
