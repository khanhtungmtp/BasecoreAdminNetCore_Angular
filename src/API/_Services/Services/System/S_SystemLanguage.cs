
using API._Repositories;
using API._Services.Interfaces.System;
using API.Helpers.Base;
using API.Helpers.Utilities;
using API.Models;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;

namespace API._Services.Services.System;
public class S_SystemLanguage(IRepositoryAccessor repoStore) : BaseServices(repoStore), I_SystemLanguage
{
    public async Task<OperationResult> CreateAsync(SystemLanguageCreateRequest request)
    {
        var functionExists = await _repoStore.SystemLanguages.FindByIdAsync(request.LanguageCode);
        if (functionExists is not null)
            return OperationResult.Conflict("System language is existed.");
        var language = new SystemLanguage()
        {
            LanguageCode = request.LanguageCode,
            LanguageName = request.LanguageName,
            UrlImage = request.UrlImage,
            IsActive = request.IsActive
        };

        await _repoStore.SystemLanguages.AddAsync(language);
        await _repoStore.SaveChangesAsync();
        return OperationResult.Success("System language created successfully.");
    }

    public async Task<OperationResult<SystemLanguageVM>> FindByCodeAsync(string languageCode)
    {
        SystemLanguage? language = await _repoStore.SystemLanguages.FindByIdAsync(languageCode);
        if (language is null)
            return OperationResult<SystemLanguageVM>.NotFound("System language not found.");
        SystemLanguageVM languageVM = new()
        {
            LanguageCode = language.LanguageCode,
            LanguageName = language.LanguageName,
            UrlImage = language.UrlImage,
            IsActive = language.IsActive,
            SortOrder = language.SortOrder
        };
        return OperationResult<SystemLanguageVM>.Success(languageVM, "Get language by id successfully.");
    }

    public async Task<OperationResult<List<SystemLanguageVM>>> GetLanguagesAsync()
    {
        var result = await _repoStore.SystemLanguages.FindAll(true).Select(x => new SystemLanguageVM() { LanguageCode = x.LanguageCode, LanguageName = x.LanguageName, UrlImage = x.UrlImage, IsActive = x.IsActive }).ToListAsync();
        return OperationResult<List<SystemLanguageVM>>.Success(result, "Get languages successfully.");
    }

    public async Task<OperationResult<PagingResult<SystemLanguageVM>>> GetPagingAsync(string? filter, PaginationParam pagination)
    {
        var query = _repoStore.SystemLanguages.FindAll(true);
        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x => x.LanguageCode.Contains(filter) || x.LanguageName.Contains(filter));
        }
        var listFunctionVM = await query.Select(x => new SystemLanguageVM()
        {
            LanguageCode = x.LanguageCode,
            LanguageName = x.LanguageName,
            UrlImage = x.UrlImage,
            IsActive = x.IsActive,
            SortOrder = x.SortOrder
        }).ToListAsync();
        var resultPaging = PagingResult<SystemLanguageVM>.Create(listFunctionVM, pagination.PageNumber, pagination.PageSize);
        return OperationResult<PagingResult<SystemLanguageVM>>.Success(resultPaging, "Get language successfully.");
    }

    public async Task<OperationResult> PutAsync(string languageCode, SystemLanguageCreateRequest request)
    {
        SystemLanguage? language = await _repoStore.SystemLanguages.FindByIdAsync(languageCode);
        if (language is null || language.LanguageCode != request.LanguageCode)
            return OperationResult.NotFound("System language not found.");
        language.LanguageCode = request.LanguageCode;
        language.LanguageName = request.LanguageName;
        language.SortOrder = request.SortOrder;
        language.UrlImage = request.UrlImage;
        language.IsActive = request.IsActive;
        _repoStore.SystemLanguages.Update(language);
        bool result = await _repoStore.SaveChangesAsync();
        if (result)
            return OperationResult.Success("System language updated successfully.");

        return OperationResult.BadRequest("System language update failed.");
    }

    public async Task<OperationResult> PatchStatusAsync(string languageCode, bool isActive)
    {
        SystemLanguage? language = await _repoStore.SystemLanguages.FindByIdAsync(languageCode);
        if (language is null)
            return OperationResult.NotFound("System language not found.");
        language.IsActive = isActive;
        _repoStore.SystemLanguages.Update(language);
        bool result = await _repoStore.SaveChangesAsync();
        if (result)
            return OperationResult.Success("Update language status successfully.");

        return OperationResult.BadRequest("Update language status failed.");
    }

    public async Task<OperationResult<string>> DeleteAsync(string languageCode)
    {
        SystemLanguage? language = await _repoStore.SystemLanguages.FindByIdAsync(languageCode);
        if (language is null)
            return OperationResult<string>.NotFound("System language not found.");

        _repoStore.SystemLanguages.Remove(language);

        bool result = await _repoStore.SaveChangesAsync();
        if (result)
            return OperationResult<string>.Success(language.LanguageCode, "System language deleted successfully.");
        return OperationResult<string>.BadRequest("System language delete failed.");
    }


}
