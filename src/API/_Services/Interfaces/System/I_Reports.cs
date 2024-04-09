using API.Helpers.Base;
using API.Helpers.Utilities;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.System;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Reports
{
    Task<OperationResult> CreateAsync(int forumId, ReportCreateRequest request);
    Task<OperationResult> DeleteAsync(int forumId, int reportId);
    Task<OperationResult<ReportVM>> FindByIdAsync(int reportId);
    Task<OperationResult<PagingResult<ReportVM>>> GetReportsPagingAsync(string? filter, PaginationParam pagination, ReportVM reportVM);
}
