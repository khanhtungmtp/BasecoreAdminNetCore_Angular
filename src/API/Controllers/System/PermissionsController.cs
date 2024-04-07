using API._Services.Interfaces.System;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.System;

public class PermissionsController(I_Permissions permissionService) : BaseController
{
    private readonly I_Permissions _permissionService = permissionService;

    //
    // Summary:
    //  Show list function with corressponding action included in each functions
    // Returns:
    // List<PermissionScreenVm> 
    [HttpGet]
    public async Task<IActionResult> GetPermissions()
    {
        return Ok(await _permissionService.GetCommandViews());
    }
}
