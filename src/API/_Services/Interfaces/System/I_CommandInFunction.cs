
using API.Helpers.Base;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.System;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_CommandInFunction
{
    Task<ApiResponse<List<CommandVm>>> GetCommandInFunction(string functionId);
    // PostCommandInFunction
    Task<ApiResponse<CommandInFunctionResponseVM>> PostCommandInFunction(string functionId, CommandAssignRequest request);
    // DeleteCommandInFunction
    Task<ApiResponse> DeleteCommandInFunction(string functionId, CommandAssignRequest request);
}
