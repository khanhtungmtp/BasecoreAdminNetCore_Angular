using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API._Repositories;
using API._Services.Interfaces.Auth;
using API.Configurations;
using API.Helpers.Base;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ViewModels.Auth;

namespace API._Services.Services.Auth;

public class S_Auth(IRepositoryAccessor repoStore, UserManager<User> userManager, IOptions<JwtSetting> jwtSettings) : BaseServices(repoStore), I_Auth
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly IOptions<JwtSetting> _jwtSettings = jwtSettings;
    public async Task<OperationResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = _repoStore.Users.FirstOrDefault(u => u.UserName == request.UserName);

        if (user is null)
            return OperationResult<AuthResponse>.BadRequest("Wrong username or password");

        bool isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
            return OperationResult<AuthResponse>.BadRequest("Wrong username or password");

        var token = GenerateToken(user);
        var refreshToken = GenerateRefreshToken();
        _ = int.TryParse(_jwtSettings.Value.RefreshTokenValidityIn, out int RefreshTokenValidityIn);
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(RefreshTokenValidityIn);
        await _userManager.UpdateAsync(user);

        var res = new AuthResponse
        {
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            Email = user.Email,
            Token = token,
            RefreshToken = refreshToken
        };
        return OperationResult<AuthResponse>.Success(res, "Login successfully");
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII
        .GetBytes(_jwtSettings.Value.SecurityKey);

        var claims = new[]
            {
               new Claim("Id", user.Id),
               new Claim(ClaimTypes.NameIdentifier, user.Id ?? ""),
               new Claim(ClaimTypes.Name, user.UserName ?? ""),
            };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);

    }

    public async Task<OperationResult<AuthResponse>> RefreshTokenAsync(TokenRequest request)
    {
        var principal = GetPrincipalFromExpiredToken(request.Token);
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (principal is null || user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return OperationResult<AuthResponse>.BadRequest("Invalid client request");

        var newJwtToken = GenerateToken(user);
        var newRefreshToken = GenerateRefreshToken();
        _ = int.TryParse(_jwtSettings.Value.RefreshTokenValidityIn, out int RefreshTokenValidityIn);

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(RefreshTokenValidityIn);

        await _userManager.UpdateAsync(user);
        return OperationResult<AuthResponse>.Success(new AuthResponse
        {
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            Email = user.Email,
            Token = newJwtToken,
            RefreshToken = newRefreshToken
        }, "Refresh token successfully");
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SecurityKey)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}
