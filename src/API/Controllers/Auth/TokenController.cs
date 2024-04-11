using API._Services.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Auth;

namespace API.Controllers.Auth;

public class TokenController : BaseController
{
    private readonly I_Token _tokenService;

    public TokenController(I_Token tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> RefreshToken(TokenRequest request)
    {
        var result = await _tokenService.RefreshTokenAsync(request);
        return HandleResult(result);
    }

    [HttpPost, Authorize]
    [Route("revoke")]
    public async Task<IActionResult> RevokeToken()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Unauthorized();

        var result = await _tokenService.RevokeTokenAsync(username);
        return HandleResult(result);
    }
}
