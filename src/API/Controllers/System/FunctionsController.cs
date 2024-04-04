using API._Services.Interfaces.System;
using API.Helpers.Base;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using ViewModels.System;

namespace API.Controllers.System;

public class FunctionsController : BaseController
{
    private readonly I_Function _functionService;
    public FunctionsController(I_Function functionService)
    {
        _functionService = functionService;
    }

    // url: POST : http://localhost:6001/api/function
    [HttpPost]
    public async Task<IActionResult> CreateUser(FunctionCreateRequest request)
    {
        var result = await _functionService.CreateAsync(request);
        if (result.Succeeded)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data }, request);
        }
        else
            return BadRequest(result.Error);
    }

    // url: GET : http:localhost:6001/api/function
    // [HttpGet]
    // public async Task<IActionResult> GetAllPaging(string filter, [FromQuery] PaginationParam pagination, FunctionVM userVM)
    // {
    //     var function = _userManager.Users;
    //     if (function is null)
    //         return NotFound();
    //     if (!string.IsNullOrWhiteSpace(filter))
    //     {
    //         bool isDate = DateTime.TryParse(filter, out DateTime filterDate);
    //         function = function.Where(x => x.FullName.Contains(filter) || x.Email != null && x.Email.Contains(filter) || (isDate && x.DateOfBirth.Date == filterDate.Date));
    //     }
    //     // more request search...
    //     var listUserVM = await function.Select(x => new FunctionVM() { Id = x.Id, FullName = x.FullName ?? string.Empty }).ToListAsync();
    //     return Ok(PagingResult<FunctionVM>.Create(listUserVM, pagination.PageNumber, pagination.PageSize));
    // }

    // // url: GET : http:localhost:6001/api/function/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var function = await _functionService.FindByIdAsync(id);
        if (!function.Succeeded)
            return NotFound(new ApiNotFoundResponse(function.Message));
        return Ok(function);
    }

    // // url: PUT : http:localhost:6001/api/function/{id}
    // [HttpPut("{id}")]
    // public async Task<IActionResult> PutUser(string id, [FromBody] FunctionCreateRequest request)
    // {
    //     var function = await _userManager.FindByIdAsync(id);
    //     if (function is null)
    //         return NotFound();
    //     function.FullName = request.FullName;
    //     function.DateOfBirth = request.DateOfBirth;
    //     function.UpdateDate = DateTime.Now;

    //     var result = await _userManager.UpdateAsync(function);
    //     if (result.Succeeded)
    //         return NoContent();
    //     return BadRequest(result.Errors);
    // }

    // // url: PUT : http:localhost:6001/api/function/{id}/change-password
    // [HttpPut("{id}/change-password")]
    // public async Task<IActionResult> ChangePassword(string id, [FromBody] UserPasswordChangeRequest request)
    // {
    //     var function = await _userManager.FindByIdAsync(id);
    //     if (function is null)
    //         return NotFound();
    //     var result = await _userManager.ChangePasswordAsync(function, request.OldPassword, request.NewPassword);
    //     if (result.Succeeded)
    //         return NoContent();
    //     return BadRequest(result.Errors);
    // }

    // // url: DELETE : http:localhost:6001/api/function/{id}
    // [HttpDelete("{id}")]
    // public async Task<IActionResult> DeleteUser(string id)
    // {
    //     var function = await _userManager.FindByIdAsync(id);
    //     if (function is null)
    //         return NotFound();
    //     var result = await _userManager.DeleteAsync(function);
    //     if (result.Succeeded)
    //     {
    //         var userVM = new FunctionVM()
    //         {
    //             Id = function.Id,
    //             UserName = function.UserName ?? string.Empty,
    //             FullName = function.FullName ?? string.Empty,
    //             DateOfBirth = function.DateOfBirth,
    //             CreateDate = function.CreateDate,
    //             Email = function.Email ?? string.Empty,
    //             PhoneNumber = function.PhoneNumber ?? string.Empty
    //         };
    //         return Ok(userVM);
    //     }
    //     return BadRequest(result.Errors);
    // }

}
