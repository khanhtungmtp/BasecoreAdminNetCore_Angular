using API._Services.Interfaces.UserManager;
using API.Helpers.Base;
using API.Helpers.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;
using ViewModels.UserManager;

namespace API.Controllers.UserManager;

[ApiController]
[Route("api/[controller]")]
public class RolesController(RoleManager<IdentityRole> rolesManager, I_Roles roles) : ControllerBase
{
    private readonly RoleManager<IdentityRole> _rolesManager = rolesManager;
    private readonly I_Roles _roles = roles;

    // url: POST : http://localhost:6001/api/roles
    [HttpPost]
    public async Task<IActionResult> CreateRole(RoleCreateRequest request)
    {
        var role = new IdentityRole()
        {
            Id = request.Id,
            Name = request.Name,
            NormalizedName = request.Name.ToUpper(),
        };
        var result = await _rolesManager.CreateAsync(role);
        if (result.Succeeded)
        {
            return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
        }
        else
            return BadRequest(new ApiBadRequestResponse(result));
    }

    // url: GET : http:localhost:6001/api/roles
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParam pagination, [FromQuery] RoleVM roleVM)
    {
        var role = _rolesManager.Roles;
        if (role is null)
            return NotFound(new ApiNotFoundResponse("Role not found"));
        if (!string.IsNullOrWhiteSpace(roleVM.Id))
        {
            role = role.Where(x => x.Id.Contains(roleVM.Id));
        }
        if (!string.IsNullOrWhiteSpace(roleVM.Name))
        {
            role = role.Where(x => x.Name != null && x.Name.Contains(roleVM.Name));
        }
        var listRoleVM = await role.Select(x => new RoleVM() { Id = x.Id, Name = x.Name ?? string.Empty }).ToListAsync();
        return Ok(PagingResult<RoleVM>.Create(listRoleVM, pagination.PageNumber, pagination.PageSize));
    }

    // url: GET : http:localhost:6001/api/roles/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var role = await _rolesManager.FindByIdAsync(id);
        if (role is null)
            return NotFound(new ApiNotFoundResponse("Role not found"));
        var roleVM = new RoleVM()
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty
        };
        return Ok(roleVM);
    }

    // url: PUT : http:localhost:6001/api/roles/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRole(string id, [FromBody] RoleCreateRequest request)
    {
        if (id != request.Id)
            return BadRequest(new ApiBadRequestResponse("Role not found"));
        var role = await _rolesManager.FindByIdAsync(id);
        if (role is null)
            return NotFound(new ApiNotFoundResponse("Role not found"));
        role.Name = request.Name;
        role.NormalizedName = request.Name.ToUpper();
        var result = await _rolesManager.UpdateAsync(role);
        if (result.Succeeded)
            return NoContent();
        return BadRequest(new ApiBadRequestResponse(result));
    }

    // url: DELETE : http:localhost:6001/api/roles/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var role = await _rolesManager.FindByIdAsync(id);
        if (role is null)
            return NotFound(new ApiNotFoundResponse("Role not found"));
        var result = await _rolesManager.DeleteAsync(role);
        if (result.Succeeded)
        {
            var roleVM = new RoleVM()
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty
            };
            return Ok(roleVM);
        }
        return BadRequest(new ApiBadRequestResponse(result));
    }

    // GetPermissionByRoleId
    // url: GET : http:localhost:6001/api/roles/{id}/permissions
    [HttpGet("{roleId}/permissions")]
    public async Task<IActionResult> GetPermissionByRoleId(string roleId)
    {
        var role = await _roles.GetPermissionByRoleId(roleId);
        if (!role.Succeeded)
            return NotFound(role);
        return Ok(role);
    }

    // PutPermissionByRoleId
    // url: PUT : http:localhost:6001/api/roles/{id}/permissions
    [HttpPut("{roleId}/permissions")]
    public async Task<IActionResult> PutPermissionByRoleId(string roleId, [FromBody] UpdatePermissionRequest request)
    {
        var role = await _roles.PutPermissionByRoleId(roleId, request);
        if (!role.Succeeded)
            return NotFound(role);
        return Ok(role);
    }

}
