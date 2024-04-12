using API.Helpers.Base;
using ViewModels.Auth;
using ViewModels.UserManager;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.Auth;

[DependencyInjection(ServiceLifetime.Transient)]
public interface I_Token
{
    Task<TokenRequest> GenerateTokenAsync(UserVM user);
    Task<OperationResult<TokenRequest>> RefreshTokenAsync(TokenRequest request);
}
