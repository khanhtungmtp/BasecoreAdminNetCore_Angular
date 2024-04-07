using API._Services.Interfaces.Forum;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Forums;

public class VotesController(I_Votes votesService, UserManager<User> userManager) : BaseController
{
    private readonly I_Votes _votesService = votesService;
    private readonly UserManager<User> _userManager = userManager;

    [HttpGet("{forumId}/votes")]
    public async Task<IActionResult> GetVotes(int forumId)
    {
        var result = await _votesService.GetVotesAsync(forumId);
        return Ok(result);
    }

    [HttpPost("{forumId}/votes")]
    public async Task<IActionResult> PostVote(int forumId)
    {
        string userId = _userManager.GetUserId(User) ?? string.Empty;
        var result = await _votesService.CreateAsync(forumId, userId);
        if (!result.Succeeded)
        {
            if (result.Status == 404)
                return NotFound(result);
            else
                return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpDelete("{forumId}/votes/{userId}")]
    public async Task<IActionResult> DeleteVote(int forumId, string userId)
    {
        var result = await _votesService.DeleteAsync(forumId, userId);
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
