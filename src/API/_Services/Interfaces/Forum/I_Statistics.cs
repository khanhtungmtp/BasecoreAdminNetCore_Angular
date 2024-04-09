
using API.Helpers.Base;
using ViewModels.Forum;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.Forums;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Statistics
{
    Task<OperationResult<List<MonthlyCommentsVM>>> GetMonthlyNewCommentsAsync(int year);
    Task<OperationResult<List<MonthlyNewKbsVM>>> GetMonthlyNewKbsAsync(int year);
    Task<OperationResult<List<MonthlyNewKbsVM>>> GetMonthlyNewRegistersAsync(int year);
}
