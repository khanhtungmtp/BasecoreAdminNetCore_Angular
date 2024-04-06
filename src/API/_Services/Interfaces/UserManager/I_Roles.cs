using API.Helpers.Base;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.UserManager;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_Roles
{
    Task<ApiResponse<List<PermissionVm>>> GetPermissionByRoleId(string roleId);
    Task<ApiResponse<string>> PutPermissionByRoleId(string roleId, UpdatePermissionRequest request);
}
