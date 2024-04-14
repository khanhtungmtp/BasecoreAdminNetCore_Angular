using System.Security.Claims;
using API._Repositories;
using API._Services.Interfaces.Auth;
using API.Helpers.Base;
using API.Models;
using Microsoft.AspNetCore.Identity;
using ViewModels.Auth;
using ViewModels.UserManager;

namespace API._Services.Services.Auth;

public class S_Auth(IRepositoryAccessor repoStore, UserManager<User> userManager, I_Token tokenService) : BaseServices(repoStore), I_Auth
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly I_Token _tokenService = tokenService;

    public async Task<OperationResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var userInDb = _repoStore.Users.FirstOrDefault(u => u.UserName == request.UserName);

        if (userInDb is null)
            return OperationResult<AuthResponse>.NotFound("Wrong username or password");

        bool isPasswordValid = await _userManager.CheckPasswordAsync(userInDb, request.Password);

        if (!isPasswordValid)
            return OperationResult<AuthResponse>.BadRequest("Wrong username or password");

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, request.UserName),
            new(ClaimTypes.Role, "Manager")
        };

        TokenRequest? token = await _tokenService.GenerateTokenAsync(new UserVM
        {
            Id = userInDb.Id,
            FullName = userInDb.FullName,
            Email = userInDb.Email ?? string.Empty,
            UserName = userInDb.UserName ?? string.Empty
        });

        userInDb.RefreshToken = token.RefreshToken;
        userInDb.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        var res = new AuthResponse
        {
            Id = userInDb.Id,
            Username = userInDb.UserName ?? string.Empty,
            Email = userInDb.Email,
            Token = token.AccessToken,
            RefreshToken = token.RefreshToken
        };
        return OperationResult<AuthResponse>.Success(res, "Login successfully");
    }
}
