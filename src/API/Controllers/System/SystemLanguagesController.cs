using API._Services.Interfaces.System;
using API.Filters.Authorization;
using API.Helpers.Constants;
using API.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels.System;

namespace API.Controllers.System;

public class SystemLanguagesController(I_SystemLanguage systemLanguageService) : BaseController
{
    private readonly I_SystemLanguage _systemLanguageService = systemLanguageService;

    // url: POST : http://localhost:6001/api/SystemLanguages
    [HttpPost]
    [ClaimRequirement(FunctionCode.SYSTEM_LANGUAGE, CommandCode.CREATE)]
    public async Task<IActionResult> PostFunction(SystemLanguageCreateRequest request)
    {
        var result = await _systemLanguageService.CreateAsync(request);
        return HandleResult(result);
    }

    // url: GET : http:localhost:6001/api/SystemLanguages/GetLanguages
    [HttpGet("GetLanguages")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLanguages()
    {
        return Ok(await _systemLanguageService.GetLanguagesAsync());
    }

    // url: GET : http:localhost:6001/api/SystemLanguages/GetAllPaging
    [HttpGet("GetAllPaging")]
    [AllowAnonymous]
    // [ClaimRequirement(FunctionCode.SYSTEM_LANGUAGE, CommandCode.VIEW)]
    public async Task<IActionResult> GetAllPaging(string? filter, [FromQuery] PaginationParam pagination)
    {
        filter ??= string.Empty;
        return Ok(await _systemLanguageService.GetPagingAsync(filter, pagination));
    }

    // url: GET : http:localhost:6001/api/SystemLanguages/{languageCode}
    [HttpGet("{languageCode}")]
    [ClaimRequirement(FunctionCode.SYSTEM_LANGUAGE, CommandCode.VIEW)]
    public async Task<IActionResult> GetById(string languageCode)
    {
        var result = await _systemLanguageService.FindByCodeAsync(languageCode);
        return HandleResult(result);
    }

    // url: PUT : http:localhost:6001/api/SystemLanguages/{id}
    [HttpPut("{id}")]
    [ClaimRequirement(FunctionCode.SYSTEM_LANGUAGE, CommandCode.UPDATE)]
    public async Task<IActionResult> PutFunction(string id, [FromBody] SystemLanguageCreateRequest request)
    {
        var result = await _systemLanguageService.PutAsync(id, request);
        return HandleResult(result);
    }

    // url: DELETE : http:localhost:6001/api/SystemLanguages/{id}
    [HttpDelete("{id}")]
    [ClaimRequirement(FunctionCode.SYSTEM_LANGUAGE, CommandCode.DELETE)]
    public async Task<IActionResult> DeleteFunction(string id)
    {
        var result = await _systemLanguageService.DeleteAsync(id);
        return HandleResult(result);
    }

}
