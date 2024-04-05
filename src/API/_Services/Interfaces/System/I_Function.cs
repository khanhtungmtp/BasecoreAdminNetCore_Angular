using API.Helpers.Base;
using API.Helpers.Utilities;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.System;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Function
{
    Task<ApiResponse<string>> CreateAsync(FunctionCreateRequest request);
    Task<ApiResponse<FunctionVM>> FindByIdAsync(string id);
    Task<PagingResult<FunctionVM>> GetAllPaging(string filter, PaginationParam pagination, FunctionVM userVM);
    Task<ApiResponse<string>> PutFunction(string id, FunctionCreateRequest request);
    Task<ApiResponse<string>> DeleteFunction(string id);
}
