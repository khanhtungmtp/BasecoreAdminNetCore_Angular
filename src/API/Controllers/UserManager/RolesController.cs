using API._Services.Interfaces.UserManager;
using API.Filters.Authorization;
using API.Helpers.Base;
using API.Helpers.Constants;
using API.Helpers.Utilities;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;
using ViewModels.UserManager;

namespace API.Controllers.UserManager;

public class RolesController(RoleManager<IdentityRole> rolesManager, I_Roles roles) : BaseController
{
    private readonly RoleManager<IdentityRole> _rolesManager = rolesManager;
    private readonly I_Roles _roles = roles;

    // url: POST : http://localhost:6001/api/roles
    [HttpPost]
    [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.CREATE)]
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
            return Ok(OperationResult.Success("Create role successfully"));
        }
        else
            return BadRequest(OperationResult.BadRequest(result.Errors));
    }

    // url: GET : http:localhost:6001/api/roles
    [HttpGet("GetAll")]
    [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.VIEW)]
    public async Task<IActionResult> GetAll()
    {
        var listRoleVM = await _rolesManager.Roles.Select(x => new RoleVM() { Id = x.Id, Name = x.Name ?? string.Empty }).ToListAsync();
        return Ok(OperationResult<List<RoleVM>>.Success(listRoleVM, "Get Roles Successfully"));
    }

    [HttpGet("GetAllPermissionTree")]
    [AllowAnonymous]
    // [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.VIEW)]
    public async Task<IActionResult> GetAllPermissionTree()
    {
        var role = await _roles.GetAllPermissionTree();
        if (!role.Succeeded)
            return NotFound(role);
        return Ok(role);
    }


    // url: GET : http:localhost:6001/api/roles
    [HttpGet("GetPaging")]
    [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.VIEW)]
    public async Task<IActionResult> GetPaging(string? filter, [FromQuery] PaginationParam pagination)
    {
        var role = _rolesManager.Roles;
        if (role is null)
            return NotFound(OperationResult.NotFound("Role not found"));
        if (!string.IsNullOrWhiteSpace(filter))
        {
            role = role.Where(x => x.Id.Contains(filter) || x.Name != null && x.Name.Contains(filter));
        }
        var listRoleVM = await role.Select(x => new RoleVM() { Id = x.Id, Name = x.Name ?? string.Empty }).ToListAsync();
        var resultPaging = PagingResult<RoleVM>.Create(listRoleVM, pagination.PageNumber, pagination.PageSize);
        return Ok(OperationResult<PagingResult<RoleVM>>.Success(resultPaging, "Get Roles Successfully"));
    }

    // url: GET : http:localhost:6001/api/roles/{id}
    [HttpGet("{id}")]
    [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.VIEW)]
    public async Task<IActionResult> GetById(string id)
    {
        var role = await _rolesManager.FindByIdAsync(id);
        if (role is null)
            return NotFound(OperationResult.NotFound("Role not found"));
        var roleVM = new RoleVM()
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty
        };
        return Ok(OperationResult<RoleVM>.Success(roleVM, "Get role successfully"));
    }

    // url: PUT : http:localhost:6001/api/roles/{id}
    [HttpPut("{id}")]
    [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.UPDATE)]
    public async Task<IActionResult> PutRole(string id, [FromBody] RoleCreateRequest request)
    {
        if (id != request.Id)
            return NotFound(OperationResult.NotFound("Role not found"));
        var role = await _rolesManager.FindByIdAsync(id);
        if (role is null)
            return NotFound(OperationResult.NotFound("Role not found"));
        role.Name = request.Name;
        role.NormalizedName = request.Name.ToUpper();
        var result = await _rolesManager.UpdateAsync(role);
        if (result.Succeeded)
            return Ok(OperationResult<string>.Success(role.Name, "Update role Successfully"));
        return BadRequest(OperationResult.BadRequest(result.Errors));
    }

    // url: DELETE : http:localhost:6001/api/roles/{id}
    [HttpDelete("{id}")]
    [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.DELETE)]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var role = await _rolesManager.FindByIdAsync(id);
        if (role is null)
            return NotFound(OperationResult.NotFound("Role not found"));
        var result = await _rolesManager.DeleteAsync(role);
        if (result.Succeeded)
            return Ok(OperationResult<string>.Success(role.Name ?? string.Empty, "Delete role Successfully"));

        return BadRequest(OperationResult.BadRequest(result.Errors));
    }

    // GetPermissionByRoleId
    // url: GET : http:localhost:6001/api/roles/{id}/permissions
    [HttpGet("{roleId}/permissions")]
    [ClaimRequirement(FunctionCode.SYSTEM_PERMISSION, CommandCode.VIEW)]
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
    [ClaimRequirement(FunctionCode.SYSTEM_PERMISSION, CommandCode.UPDATE)]
    public async Task<IActionResult> PutPermissionByRoleId(string roleId, [FromBody] List<PermissionVm> request)
    {
        var role = await _roles.PutPermissionByRoleId(roleId, request);
        if (!role.Succeeded)
            return NotFound(role);
        return Ok(role);
    }

}
