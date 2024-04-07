
using API.Helpers.Base;
using API.Helpers.Utilities;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.System;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Category
{
    Task<ApiResponse<string>> CreateAsync(CategoryCreateRequest request);
    Task<ApiResponse<CategoryVM>> FindByIdAsync(int id);
    Task<ApiResponse<PagingResult<CategoryVM>>> GetCategoriesPagingAsync(string? filter, PaginationParam pagination, CategoryVM categoryVM);
    Task<ApiResponse> PutCategoryAsync(int id, CategoryCreateRequest request);
    Task<ApiResponse<string>> DeleteCategoryAsync(int id);
}
