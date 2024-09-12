using API._Services.Interfaces.System;
using API.Helpers.Base;
using API.Helpers.Utilities;
using Microsoft.AspNetCore.Mvc;
using ViewModels.System;

namespace API.Controllers.System;

public class ReportsController(I_Reports reportService) : BaseController
{
    private readonly I_Reports _reportService = reportService;

    [HttpPost("{forumId}/reports")]
    public async Task<IActionResult> PostReport(int forumId, [FromBody] ReportCreateRequest request)
    {
        OperationResult? result = await _reportService.CreateAsync(forumId, request);
        return HandleResult(result);
    }

    [HttpGet("{forumId}/reports/filter")]
    public async Task<IActionResult> GetReportsPaging(string? filter, [FromQuery] PaginationParam pagination, ReportVM reportVM)
    {
        var result = await _reportService.GetPagingAsync(filter, pagination, reportVM);
        return Ok(result);
    }

    [HttpGet("{forumId}/reports/{reportId}")]
    public async Task<IActionResult> GetReportDetail(int reportId)
    {
        OperationResult<ReportVM>? result = await _reportService.FindByIdAsync(reportId);
        return Ok(result);
    }

    [HttpDelete("{forumId}/reports/{reportId}")]
    public async Task<IActionResult> DeleteReport(int forumId, int reportId)
    {
        OperationResult? result = await _reportService.DeleteAsync(forumId, reportId);
        return HandleResult(result);
    }
}
