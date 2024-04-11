using System.Security.Claims;
using API.Helpers.Base;
using API.Models;
using ViewModels.Auth;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.Auth;

[DependencyInjection(ServiceLifetime.Transient)]
public interface I_Token
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    Task<OperationResult<AuthResponse>> RefreshTokenAsync(TokenRequest refreshToken);
    Task<OperationResult> RevokeTokenAsync(string userName);
}
