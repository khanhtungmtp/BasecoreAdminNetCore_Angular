using API._Services.Interfaces.Forum;
using API.Helpers.Utilities;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Forum;

namespace API.Controllers.Forum;

public class ForumsController(UserManager<User> userManager, I_Forums forumService) : BaseController
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly I_Forums _forumService = forumService;

    // post forum
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> PostForum([FromForm] ForumCreateRequest request)
    {
        request.OwnerUserId = _userManager.GetUserId(User) ?? string.Empty;
        var result = await _forumService.CreateAsync(request);
        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
    // get all forums
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllForums(string? filter, [FromQuery] PaginationParam pagination, [FromQuery] ForumQuickVM forumVM)
    {

        var result = await _forumService.GetForumsPagingAsync(filter, pagination, forumVM);
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("latest/{take:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLatestForums(int take)
    {
        var result = await _forumService.GetLatestForumAsync(take);
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("popular/{take:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPopularForums(int take)
    {
        var result = await _forumService.GetPopularForumAsync(take);
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("tags/{labelId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetForumByTagId(string labelId, PaginationParam pagination)
    {
        var result = await _forumService.GetForumByTagIdAsync(labelId, pagination);
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> PutForum(int id, [FromForm] ForumCreateRequest request)
    {
        var result = await _forumService.PutAsync(id, request);
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteForum(int id)
    {
        var result = await _forumService.DeleteAsync(id);
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{forumId}/labels")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLabelsByForumId(int forumId)
    {
        var result = await _forumService.GetLabelsByForumIdAsync(forumId);
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{id}/view-count")]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateViewCount(int id)
    {
        var result = await _forumService.UpdateViewCountAsync(id);
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }


}
