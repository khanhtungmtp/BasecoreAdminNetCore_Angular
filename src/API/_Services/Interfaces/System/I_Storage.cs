using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.System;

[DependencyInjection(ServiceLifetime.Transient)]
public interface I_Storage
{
    string GetFileUrl(string fileName);

    Task SaveFileAsync(Stream mediaBinaryStream, string fileName);

    Task DeleteFileAsync(string fileName);
}
