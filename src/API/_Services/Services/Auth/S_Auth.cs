using System.Security.Claims;
using API._Repositories;
using API._Services.Interfaces.Auth;
using API.Helpers.Base;
using API.Models;
using Microsoft.AspNetCore.Identity;
using ViewModels.Auth;

namespace API._Services.Services.Auth;

public class S_Auth : BaseServices, I_Auth
{
    private readonly UserManager<User> _userManager;
    private readonly I_Token _tokenService;

    public S_Auth(IRepositoryAccessor repoStore, UserManager<User> userManager, I_Token tokenService) : base(repoStore)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<OperationResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var userInDb = _repoStore.Users.FirstOrDefault(u => u.UserName == request.UserName);

        if (userInDb is null)
            return OperationResult<AuthResponse>.NotFound("Wrong username or password");

        var isPasswordValid = await _userManager.CheckPasswordAsync(userInDb, request.Password);

        if (!isPasswordValid)
            return OperationResult<AuthResponse>.BadRequest("Wrong username or password");
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, request.UserName),
            new(ClaimTypes.Role, "Manager")
        };
        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();
        userInDb.RefreshToken = refreshToken;
        userInDb.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        await _repoStore.SaveChangesAsync();
        var res = new AuthResponse
        {
            Username = userInDb.UserName ?? string.Empty,
            Email = userInDb.Email,
            Token = accessToken,
            RefreshToken = refreshToken
        };
        return OperationResult<AuthResponse>.Success(res);
    }
}
