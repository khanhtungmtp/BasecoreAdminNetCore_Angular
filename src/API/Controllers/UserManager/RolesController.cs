using API.Helpers.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViewModels.UserManager;

namespace API.Controllers.UserManager;

[ApiController]
[Route("api/[controller]")]
public class RolesController(RoleManager<IdentityRole> rolesManager) : ControllerBase
{
    private readonly RoleManager<IdentityRole> _rolesManager = rolesManager;

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
            return BadRequest(result.Errors);
    }

    // url: GET : http:localhost:6001/api/roles
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParam pagination, RoleVM roleVM)
    {
        var role = _rolesManager.Roles;
        if (role is null)
            return NotFound();
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
            return NotFound();
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (id != request.Id)
            return BadRequest();
        var role = await _rolesManager.FindByIdAsync(id);
        if (role is null)
            return NotFound();
        role.Name = request.Name;
        role.NormalizedName = request.Name.ToUpper();
        var result = await _rolesManager.UpdateAsync(role);
        if (result.Succeeded)
            return NoContent();
        return BadRequest(result.Errors);
    }

    // url: DELETE : http:localhost:6001/api/roles/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var role = await _rolesManager.FindByIdAsync(id);
        if (role is null)
            return NotFound();
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
        return BadRequest(result.Errors);
    }

}
