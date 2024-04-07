using API._Services.Interfaces.System;
using API.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels.System;

namespace API.Controllers.System;

public class CategoryController(I_Category categoryService) : BaseController
{
    private readonly I_Category _categoryService = categoryService;

    [HttpPost]
    public async Task<IActionResult> PostCategory([FromBody] CategoryCreateRequest request)
    {
        var result = await _categoryService.CreateAsync(request);
        if (result.Succeeded)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data }, request);
        }
        return BadRequest(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _categoryService.FindByIdAsync(id);
        if (!result.Succeeded)
            return NotFound(result);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetCategoriesPaging(string? filter, PaginationParam pagination, [FromQuery] CategoryVM categoryVM)
    {
        var result = await _categoryService.GetCategoriesPagingAsync(filter, pagination, categoryVM);
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCategory(int id, [FromBody] CategoryCreateRequest request)
    {
        var result = await _categoryService.PutCategoryAsync(id, request);
        if (!result.Succeeded)
        {
            if (result.Status == 404)
                return NotFound(result);
            else
                return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        if (!result.Succeeded)
        {
            if (result.Status == 404)
                return NotFound(result);
            else
                return BadRequest(result);
        }
        return Ok(result);
    }
}
