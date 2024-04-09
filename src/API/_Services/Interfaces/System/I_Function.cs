using API.Helpers.Base;
using API.Helpers.Utilities;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.System;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Function
{
    Task<OperationResult<string>> CreateAsync(FunctionCreateRequest request);
    Task<OperationResult<FunctionVM>> FindByIdAsync(string id);
    Task<OperationResult<PagingResult<FunctionVM>>> GetAllPaging(string? filter, PaginationParam pagination, FunctionVM functionVM);
    Task<OperationResult<string>> PutFunctionAsync(string id, FunctionCreateRequest request);
    Task<OperationResult<string>> DeleteFunctionAsync(string id);
}
