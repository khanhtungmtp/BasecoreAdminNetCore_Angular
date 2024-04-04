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
            DateOfBirth = request.DateOfBirth
        };
        IdentityResult? result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, request);
        }
        else
            return BadRequest(result.Errors);
    }

    // url: GET : http:localhost:6001/api/user
    [HttpGet]
    public async Task<IActionResult> GetAllPaging(string filter, [FromQuery] PaginationParam pagination, UserVM userVM)
    {
        var user = _userManager.Users;
        if (user is null)
            return NotFound();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            bool isDate = DateTime.TryParse(filter, out DateTime filterDate);
            user = user.Where(x => x.FullName.Contains(filter) || x.Email != null && x.Email.Contains(filter) || (isDate && x.DateOfBirth.Date == filterDate.Date));
        }
        // more request search...
        var listUserVM = await user.Select(x => new UserVM() { Id = x.Id, FullName = x.FullName ?? string.Empty }).ToListAsync();
        return Ok(PagingResult<UserVM>.Create(listUserVM, pagination.PageNumber, pagination.PageSize));
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
        user.UpdateDate = DateTime.Now;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
            return NoContent();
        return BadRequest(result.Errors);
    }

    // url: PUT : http:localhost:6001/api/user/{id}/change-password
    [HttpPut("{id}/change-password")]
    public async Task<IActionResult> ChangePassword(string id, [FromBody] UserPasswordChangeRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();
        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
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
                UserName = user.UserName ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                DateOfBirth = user.DateOfBirth,
                CreateDate = user.CreateDate,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty
            };
            return Ok(userVM);
        }
        return BadRequest(result.Errors);
    }

}
