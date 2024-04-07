
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.System;

[DependencyInjection(ServiceLifetime.Transient)]
public interface I_Cache
{
    Task<T?> GetAsync<T>(string key);

    Task SetAsync<T>(string key, T value, int timeDurationInHours = 0);

    Task RemoveAsync(string key);
}
