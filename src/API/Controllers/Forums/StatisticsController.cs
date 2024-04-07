using API._Services.Interfaces.Forums;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Forums;

public class StatisticsController(I_Statistics statisticsService) : BaseController
{
    private readonly I_Statistics _statisticsService = statisticsService;

    [HttpGet("monthly-comments")]
    public async Task<IActionResult> GetMonthlyNewComments(int year)
    {
        var result = await _statisticsService.GetMonthlyNewCommentsAsync(year);
        return Ok(result);
    }

    [HttpGet("monthly-newkbs")]
    public async Task<IActionResult> GetMonthlyNewKbs(int year)
    {
        var result = await _statisticsService.GetMonthlyNewKbsAsync(year);
        return Ok(result);
    }

    [HttpGet("monthly-registers")]
    public async Task<IActionResult> GetMonthlyNewRegisters(int year)
    {
        var result = await _statisticsService.GetMonthlyNewRegistersAsync(year);
        return Ok(result);
    }

}
