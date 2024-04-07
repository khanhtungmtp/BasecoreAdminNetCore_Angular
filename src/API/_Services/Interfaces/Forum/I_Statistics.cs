
using API.Helpers.Base;
using ViewModels.Forum;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.Forums;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Statistics
{
    Task<ApiResponse<List<MonthlyCommentsVM>>> GetMonthlyNewCommentsAsync(int year);
    Task<ApiResponse<List<MonthlyNewKbsVM>>> GetMonthlyNewKbsAsync(int year);
    Task<ApiResponse<List<MonthlyNewKbsVM>>> GetMonthlyNewRegistersAsync(int year);
}
