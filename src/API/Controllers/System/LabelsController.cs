using API._Services.Interfaces.System;
using API.Helpers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels.System;

namespace API.Controllers.System;

public class LabelsController(I_Labels labelService) : BaseController
{
    private readonly I_Labels _labelService = labelService;

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(string id)
    {
        OperationResult<LabelVM>? result = await _labelService.FindByIdAsync(id);
        return HandleResult(result);
    }

    [HttpGet("popular/{take:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPopularLabels(int take)
    {
        OperationResult<List<LabelVM>>? result = await _labelService.GetPopularLabelsAsync(take);
        return HandleResult(result);
    }
}
