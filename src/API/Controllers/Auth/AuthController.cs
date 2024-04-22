using API._Services.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Auth;
namespace API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController(I_Auth authService) : ControllerBase
{
    private readonly I_Auth _authService = authService;

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (!result.Succeeded)
        {
            if (result.StatusCode == 401)
                return Unauthorized(result);
            return BadRequest(result);
        }
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponse>> RefreshToken(TokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
}
