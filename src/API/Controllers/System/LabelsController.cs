using API._Services.Interfaces.System;
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
        var result = await _labelService.FindByIdAsync(id);
        if (!result.Succeeded)
        {
            if (result.Status == 404)
                return NotFound(result);
            else
                return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("popular/{take:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPopularLabels(int take)
    {
        var result = await _labelService.GetPopularLabelsAsync(take);
        if (!result.Succeeded)
            return NotFound(result);
        return Ok(result);
    }
}
