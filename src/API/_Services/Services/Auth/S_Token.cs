using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API._Repositories;
using API._Services.Interfaces.Auth;
using API.Configurations;
using API.Helpers.Base;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ViewModels.Auth;

namespace API._Services.Services.Auth;

public class S_Token : BaseServices, I_Token
{
    public S_Token(IRepositoryAccessor repoStore, IOptions<JwtTokenSettings> jwtTokenSettings) : base(repoStore)
    {
        _jwtTokenSettings = jwtTokenSettings.Value;
    }
    private readonly JwtTokenSettings _jwtTokenSettings;
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenSettings.SymmetricSecurityKey));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokeOptions = new JwtSecurityToken(
            issuer: _jwtTokenSettings.ValidIssuer,
            audience: _jwtTokenSettings.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: signinCredentials
        );
        string? tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return tokenString;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenSettings.SymmetricSecurityKey)),
            ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }

    public async Task<OperationResult<AuthResponse>> RefreshTokenAsync(TokenRequest request)
    {
        if (request is null)
            return OperationResult<AuthResponse>.BadRequest("Invalid client request");
        string accessToken = request.AccessToken;
        string refreshToken = request.RefreshToken;
        var principal = GetPrincipalFromExpiredToken(accessToken);
        string? username = principal.Identity?.Name; //this is mapped to the Name claim by default
        var user = await _repoStore.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            return OperationResult<AuthResponse>.BadRequest("Invalid client request");
        string? newAccessToken = GenerateAccessToken(principal.Claims);
        string? newRefreshToken = GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        await _repoStore.SaveChangesAsync();
        return OperationResult<AuthResponse>.Success(new AuthResponse()
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    public async Task<OperationResult> RevokeTokenAsync(string userName)
    {
        var user = await _repoStore.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        if (user is null) return OperationResult.NotFound("User not found");
        user.RefreshToken = null;
       await _repoStore.SaveChangesAsync();
        return OperationResult.Success("Revoke token successfully");
    }
}
