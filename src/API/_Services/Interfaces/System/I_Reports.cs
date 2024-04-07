using API.Helpers.Base;
using API.Helpers.Utilities;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.System;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Reports
{
    Task<ApiResponse> CreateAsync(int forumId, ReportCreateRequest request);
    Task<ApiResponse> DeleteAsync(int forumId, int reportId);
    Task<ApiResponse<ReportVM>> FindByIdAsync(int reportId);
    Task<ApiResponse<PagingResult<ReportVM>>> GetReportsPagingAsync(string? filter, PaginationParam pagination, ReportVM reportVM);
}
