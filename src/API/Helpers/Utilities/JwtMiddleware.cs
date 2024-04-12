using System.IdentityModel.Tokens.Jwt;
using System.Text;
using API._Services.Interfaces.UserManager;
using API.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Helpers.Utilities;

public class JwtMiddleware(RequestDelegate next, IOptions<AppSetting> appSettings)
{
    private readonly RequestDelegate _next = next;
    private readonly AppSetting _appSetting = appSettings.Value;

    public async Task Invoke(HttpContext context, I_User userService)
    {
        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

        if (token is not null)
            await AttachUserToContext(context, userService, token);

        await _next(context);
    }

    private async Task AttachUserToContext(HttpContext context, I_User userService, string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSetting.SecretKey);
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            // set clock skew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var userId = jwtToken.Claims.First(x => x.Type == "Id").Value;

        //Attach user to context on successful JWT validation
        context.Items["User"] = await userService.GetById(userId);
    }
}