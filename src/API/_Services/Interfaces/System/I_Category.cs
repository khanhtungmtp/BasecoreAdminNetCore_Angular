
using API.Helpers.Base;
using API.Helpers.Utilities;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.System;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Category
{
    Task<OperationResult<string>> CreateAsync(CategoryCreateRequest request);
    Task<OperationResult<CategoryVM>> FindByIdAsync(int id);
    Task<OperationResult<PagingResult<CategoryVM>>> GetPagingAsync(string? filter, PaginationParam pagination, CategoryVM categoryVM);
    Task<OperationResult> PutAsync(int id, CategoryCreateRequest request);
    Task<OperationResult<string>> DeleteAsync(int id);
}
