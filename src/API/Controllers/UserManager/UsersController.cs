using API._Services.Interfaces.UserManager;
using API.Helpers.Base;
using API.Helpers.Utilities;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViewModels.UserManager;

namespace API.Controllers.UserManager;

[ApiController]
[Route("api/[controller]")]
public class UsersController(UserManager<User> userManager, I_User user) : BaseController
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly I_User _user = user;

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
            return BadRequest(OperationResult.BadRequest(result.Errors));
    }

    // url: PUT : http:localhost:6001/api/user/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(string id, [FromBody] UserCreateRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound(OperationResult.NotFound("User not found"));
        user.FullName = request.FullName;
        user.PasswordHash = request.Password;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;
        user.DateOfBirth = request.DateOfBirth;
        user.UpdatedDate = DateTime.Now;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
            return Ok(OperationResult<string>.Success(user.UserName ?? string.Empty, "Update user successfully"));
        return BadRequest(OperationResult.BadRequest(result.Errors));
    }

    // url: GET : http:localhost:6001/api/user
    [HttpGet]
    public async Task<IActionResult> GetPaging(string? filter, [FromQuery] PaginationParam pagination, [FromQuery] UserVM userVM)
    {
        var user = _userManager.Users;
        if (user is null)
            return NotFound(OperationResult.NotFound("User not found"));
        if (!string.IsNullOrWhiteSpace(filter))
        {
            bool isDate = DateTime.TryParse(filter, out DateTime filterDate);
            user = user.Where(x => x.FullName.Contains(filter) || x.Email != null && x.Email.Contains(filter) || (isDate && x.DateOfBirth.Date == filterDate.Date));
        }
        // more request search...
        var listUserVM = await user.Select(x => new UserVM()
        {
            Id = x.Id,
            FullName = x.FullName,
            UserName = x.UserName ?? string.Empty,
            Email = x.Email ?? string.Empty,
            PhoneNumber = x.PhoneNumber ?? string.Empty,
            DateOfBirth = x.DateOfBirth,
            CreatedDate = x.CreatedDate
        }).ToListAsync();
        var resultPaging = PagingResult<UserVM>.Create(listUserVM, pagination.PageNumber, pagination.PageSize);
        return Ok(OperationResult<PagingResult<UserVM>>.Success(resultPaging, "Get Users Successfully"));
    }

    // url: GET : http:localhost:6001/api/user/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound(OperationResult.NotFound("User not found"));
        var userVM = new UserVM()
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            DateOfBirth = user.DateOfBirth,
            CreatedDate = user.CreatedDate

        };
        return Ok(OperationResult<UserVM>.Success(userVM, "Get Users Successfully"));
    }

    // url: PUT : http:localhost:6001/api/user/{id}/change-password
    [HttpPut("{id}/change-password")]
    public async Task<IActionResult> ChangePassword(string id, [FromBody] UserPasswordChangeRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound(OperationResult.NotFound("User not found"));
        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (result.Succeeded)
            return Ok(OperationResult.Success("Change password successfully"));
        return BadRequest(OperationResult.BadRequest(result.Errors));
    }

    // url: DELETE : http:localhost:6001/api/user/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound(OperationResult.NotFound("User not found"));
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            var userVM = new UserVM()
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                DateOfBirth = user.DateOfBirth,
                CreatedDate = user.CreatedDate,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty
            };
            return Ok(OperationResult<UserVM>.Success(userVM, "Get Users Successfully"));
        }
        return BadRequest(OperationResult.BadRequest(result.Errors));
    }

    // GetMenuByUserPermission
    [HttpGet("{userId}/menu")]
    public async Task<IActionResult> GetMenuByUserPermission(string userId)
    {
        var result = await _user.GetMenuByUserPermission(userId);
        return HandleResult(result);
    }

}
