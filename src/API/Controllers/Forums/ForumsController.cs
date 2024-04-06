using API._Services.Interfaces.Forum;
using API.Helpers.Utilities;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Forum;

namespace API.Controllers.Forum;

public class ForumsController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly I_Forums _forumService;

    public ForumsController(UserManager<User> userManager, I_Forums forumService)
    {
        _userManager = userManager;
        _forumService = forumService;
    }

    // post forum
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> PostForum([FromForm] ForumCreateRequest request)
    {
        request.OwnerUserId = _userManager.GetUserId(User) ?? string.Empty;
        var result = await _forumService.PostForumAsync(request);
        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
    // get all forums
    [HttpGet]
    public async Task<IActionResult> GetAllForums(string? filter, [FromQuery] PaginationParam pagination, [FromQuery] ForumQuickVM forumVM)
    {

        var result = await _forumService.GetForumsPagingAsync(filter, pagination, forumVM);
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }
}
