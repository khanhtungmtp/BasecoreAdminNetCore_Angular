
using API.Helpers.Base;
using API.Helpers.Utilities;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.System;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_SystemLanguage
{
    Task<OperationResult> CreateAsync(SystemLanguageCreateRequest request);
    Task<OperationResult<SystemLanguageVM>> FindByCodeAsync(string languageCode);
    Task<OperationResult<List<SystemLanguageVM>>> GetLanguagesAsync();
    Task<OperationResult<PagingResult<SystemLanguageVM>>> GetPagingAsync(string? filter, PaginationParam pagination);
    Task<OperationResult> PutAsync(string languageCode, SystemLanguageCreateRequest request);
    Task<OperationResult<string>> DeleteAsync(string languageCode);
}
