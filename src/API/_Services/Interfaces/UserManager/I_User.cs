using API.Helpers.Base;
using API.Models;
using ViewModels.System;
using static API.Configurations.DependencyInjectionConfig;

namespace API._Services.Interfaces.UserManager;

[DependencyInjection(ServiceLifetime.Scoped)]
public interface I_User
{
    // GetMenuByUserPermission
    Task<OperationResult<List<FunctionVM>>> GetMenuByUserPermission(string userId);
    Task<User?> GetById(string id);
}
