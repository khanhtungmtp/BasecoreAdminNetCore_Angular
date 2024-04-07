using API.Helpers.Base;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.System;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Labels
{
    Task<ApiResponse<LabelVM>> FindByIdAsync(string id);
    Task<ApiResponse<List<LabelVM>>> GetPopularLabelsAsync(int take);
}
