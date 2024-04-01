using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ViewModels.UserManager;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController(RoleManager<IdentityRole> rolesManager) : ControllerBase
{
    private readonly RoleManager<IdentityRole> _rolesManager = rolesManager;

    // url: POST : http://localhost:6001/api/roles
    [HttpPost]
    public async Task<IActionResult> CreateRole(RoleVM roleVM)
    {
        var role = new IdentityRole()
        {
            Id = roleVM.Id,
            Name = roleVM.Name,
            NormalizedName = roleVM.Name.ToUpper(),
        };
        var result = await _rolesManager.CreateAsync(role);
        if (result.Succeeded)
        {
            return CreatedAtAction(nameof(GetById), new { id = roleVM.Id }, roleVM);
        }
        else
            return BadRequest(result.Errors);
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
    public async Task<IActionResult> PutRole(string id, [FromBody] RoleVM roleVM)
    {
        if (id != roleVM.Id)
            return BadRequest();
        var role = await _rolesManager.FindByIdAsync(id);
        if (role is null)
            return NotFound();
        role.Name = roleVM.Name;
        role.NormalizedName = roleVM.Name.ToUpper();
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
