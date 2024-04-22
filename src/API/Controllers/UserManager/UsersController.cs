using API._Services.Interfaces.UserManager;
using API.Filters.Authorization;
using API.Helpers.Base;
using API.Helpers.Constants;
using API.Helpers.Utilities;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ViewModels.UserManager;

namespace API.Controllers.UserManager;

public class UsersController(UserManager<User> userManager, I_User user) : BaseController
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly I_User _user = user;

    // url: POST : http://localhost:6001/api/user
    [HttpPost]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.CREATE)]
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
        if (!result.Succeeded)
        {
            return BadRequest(OperationResult.BadRequest(result.Errors));
        }
        if (request.Roles is null)
        {
            await _userManager.AddToRoleAsync(user, "User");
        }
        else
        {
            foreach (string? role in request.Roles)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        return Ok(OperationResult.Success("Create user successfully"));
    }

    // url: PUT : http:localhost:6001/api/user/{id}
    [HttpPut("{id}")]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.UPDATE)]
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
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
    public async Task<IActionResult> GetPaging([FromQuery] PaginationParam pagination, [FromQuery] UserSearchRequest userSearchRequest)
    {
        return Ok(await _user.GetPaging(pagination, userSearchRequest));
    }

    // url: GET : http:localhost:6001/api/user/{id}
    [HttpGet("{id}")]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _user.GetByIdAsync(id);
        return HandleResult(user);
    }

    // url: PUT : http:localhost:6001/api/user/{id}/change-password
    [HttpPut("{id}/change-password")]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.UPDATE)]
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
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.DELETE)]
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
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
    public async Task<IActionResult> GetMenuByUserPermission(string userId)
    {
        var result = await _user.GetMenuByUserPermission(userId);
        return HandleResult(result);
    }

    [HttpGet("{userId}/roles")]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound(OperationResult.NotFound($"Cannot found user with id: {userId}"));
        var roles = await _userManager.GetRolesAsync(user);
        return Ok(OperationResult<IList<string>>.Success(roles, "Get user roles successfully"));
    }

    [HttpPost("{userId}/roles")]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.UPDATE)]
    public async Task<IActionResult> PostRolesToUserUser(string userId, [FromBody] RoleAssignRequest request)
    {
        if (request.RoleNames.Length == 0)
            return BadRequest(OperationResult.BadRequest("Role names cannot empty"));
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound(OperationResult.NotFound($"Cannot found user with id: {userId}"));
        var result = await _userManager.AddToRolesAsync(user, request.RoleNames);
        if (result.Succeeded)
            return Ok(OperationResult.Success("Add roles to user successfully"));

        return BadRequest(OperationResult.BadRequest(result.Errors));
    }

    [HttpDelete("{userId}/roles")]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
    public async Task<IActionResult> RemoveRolesFromUser(string userId, [FromQuery] RoleAssignRequest request)
    {
        if (request.RoleNames.Length == 0)
            return BadRequest(OperationResult.BadRequest("Role names cannot empty"));
        if (request.RoleNames.Length == 1 && request.RoleNames[0] == SystemConstants.Roles.Admin)
            return BadRequest(OperationResult.BadRequest($"Cannot remove {SystemConstants.Roles.Admin} role"));
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound(OperationResult.NotFound($"Cannot found user with id: {userId}"));
        var result = await _userManager.RemoveFromRolesAsync(user, request.RoleNames);
        if (result.Succeeded)
            return Ok(OperationResult.Success("Remove roles from user successfully"));

        return BadRequest(OperationResult.BadRequest(result.Errors));
    }

    [HttpGet("{userId}/Forum")]
    public async Task<IActionResult> GetForumByUserId(string userId, PaginationParam pagination)
    {
        return Ok(await _user.GetForumByUserId(userId, pagination));
    }
}

