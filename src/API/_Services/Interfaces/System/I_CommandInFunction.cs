
using API.Helpers.Base;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.System;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_CommandInFunction
{
    Task<ApiResponse<List<CommandVM>>> FindIdsCommandInFunctionAsync(string functionId);
    Task<ApiResponse<CommandInFunctionResponseVM>> CreateAsync(string functionId, CommandAssignRequest request);
    Task<ApiResponse> DeleteAsync(string functionId, CommandAssignRequest request);
}
