using API._Services.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Auth;
namespace API.Controllers.Auth;

public class AuthController(I_Auth authService) : ControllerBase
{
    private readonly I_Auth _authService = authService;

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var res = await _authService.LoginAsync(request);
        if (!res.Succeeded)
        {
            if (res.StatusCode == 401)
                return Unauthorized(res);
            return BadRequest(res);
        }
        return Ok(res);
    }
}
