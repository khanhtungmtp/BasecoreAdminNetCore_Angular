using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.System;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Sequence
{
    Task<int> GetNextSequenceValueAsync();
}
