using API.Helpers.Utilities;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViewModels.UserManager;

namespace API.Controllers.UserManager;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public UsersController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    // url: POST : http://localhost:6001/api/user
    [HttpPost]
    public async Task<IActionResult> CreateUser(UserCreateRequest request)
    {
        var user = new User()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = request.UserName,
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
        };
        var result = await _userManager.CreateAsync(user);
        if (result.Succeeded)
        {
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, request);
        }
        else
            return BadRequest(result.Errors);
    }

    // url: GET : http:localhost:6001/api/user
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParam pagination, UserVM userVM)
    {
        var user = _userManager.Users;
        if (user is null)
            return NotFound();
        if (!string.IsNullOrWhiteSpace(userVM.Id))
        {
            user = user.Where(x => x.Id.Contains(userVM.Id));
        }
        if (!string.IsNullOrWhiteSpace(userVM.FullName))
        {
            user = user.Where(x => x.FullName.Contains(userVM.FullName));
        }
        // more request search...
        var listUserVM = await user.Select(x => new UserVM() { Id = x.Id, FullName = x.FullName ?? string.Empty }).ToListAsync();
        return Ok(PaginationUtility<UserVM>.Create(listUserVM, pagination.PageNumber, pagination.PageSize));
    }

    // url: GET : http:localhost:6001/api/user/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();
        var userVM = new UserVM()
        {
            Id = user.Id,
            FullName = user.FullName ?? string.Empty
        };
        return Ok(userVM);
    }

    // url: PUT : http:localhost:6001/api/user/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(string id, [FromBody] UserCreateRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();
        user.FullName = request.FullName;
        user.DateOfBirth = request.DateOfBirth;
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
            return NoContent();
        return BadRequest(result.Errors);
    }

    // url: DELETE : http:localhost:6001/api/user/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            var userVM = new UserVM()
            {
                Id = user.Id,
                FullName = user.FullName ?? string.Empty
            };
            return Ok(userVM);
        }
        return BadRequest(result.Errors);
    }

}
