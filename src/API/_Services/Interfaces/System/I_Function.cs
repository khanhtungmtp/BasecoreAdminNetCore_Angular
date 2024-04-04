using API.Helpers.Base;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.System;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Function
{
    Task<ApiResponse<string>> CreateAsync(FunctionCreateRequest request);
    Task<ApiResponse<FunctionVM>> FindByIdAsync(string id);
}
