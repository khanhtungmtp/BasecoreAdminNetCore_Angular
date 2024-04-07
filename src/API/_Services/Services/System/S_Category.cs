using System.Net;
using API._Repositories;
using API._Services.Interfaces.System;
using API.Helpers.Base;
using API.Helpers.Utilities;
using API.Models;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;

namespace API._Services.Services.System;
public class S_Category(IRepositoryAccessor repoStore, I_Cache cacheService) : BaseServices(repoStore), I_Category
{
    private readonly I_Cache _cacheService = cacheService;

    public async Task<ApiResponse<string>> CreateAsync(CategoryCreateRequest request)
    {
        var category = new Category()
        {
            Name = request.Name,
            ParentId = request.ParentId,
            SortOrder = request.SortOrder,
            SeoAlias = request.SeoAlias,
            SeoDescription = request.SeoDescription
        };
        _repoStore.Categories.Add(category);
        var result = await _repoStore.SaveChangesAsync();

        if (result)
        {
            await _cacheService.RemoveAsync("Categories");

            return new ApiResponse<string>((int)HttpStatusCode.OK, true, "Create category successfully", category.Id.ToString());
        }
        else
            return new ApiResponse<string>((int)HttpStatusCode.BadRequest, false, "Create category failed", null!);

    }

    public async Task<ApiResponse<PagingResult<CategoryVM>>> GetCategoriesPagingAsync(string? filter, PaginationParam pagination, CategoryVM categoryVM)
    {
        var query = _repoStore.Categories.FindAll(true);
        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x => x.SeoDescription.Contains(filter) || x.Name.Contains(filter));
        }
        var listCategoryVM = await query.Select(x => CreateCategoryVM(x)).ToListAsync();
        var resultPaging = PagingResult<CategoryVM>.Create(listCategoryVM, pagination.PageNumber, pagination.PageSize);
        return new ApiResponse<PagingResult<CategoryVM>>((int)HttpStatusCode.OK, true, "Get function successfully.", resultPaging);
    }

    public async Task<ApiResponse<CategoryVM>> FindByIdAsync(int id)
    {
        var category = await _repoStore.Categories.FindAsync(id);
        if (category is null)
            return new ApiResponse<CategoryVM>((int)HttpStatusCode.NotFound, false, $"Cannot found category with id: {id}", null!);

        CategoryVM categoryVM = CreateCategoryVM(category);

        return new ApiResponse<CategoryVM>((int)HttpStatusCode.OK, true, $"Get category by id {id} successfully.", categoryVM);
    }

    private static CategoryVM CreateCategoryVM(Category category)
    {
        return new CategoryVM()
        {
            Id = category.Id,
            Name = category.Name,
            ParentId = category.ParentId,
            SortOrder = category.SortOrder,
            NumberOfTickets = category.NumberOfTickets,
            SeoAlias = category.SeoAlias,
            SeoDescription = category.SeoDescription
        };
    }

    public async Task<ApiResponse> PutCategoryAsync(int id, CategoryCreateRequest request)
    {
        var category = await _repoStore.Categories.FindAsync(id);
        if (category is null)
            return new ApiNotFoundResponse($"Cannot found category with id: {id}");

        if (id == request.ParentId)
            return new ApiBadRequestResponse("Category cannot be a child itself.");

        category.Name = request.Name;
        category.ParentId = request.ParentId;
        category.SortOrder = request.SortOrder;
        category.SeoDescription = request.SeoDescription;
        category.SeoAlias = request.SeoAlias;

        _repoStore.Categories.Update(category);
        var result = await _repoStore.SaveChangesAsync();

        if (result)
        {
            await _cacheService.RemoveAsync("Categories");

            return new ApiResponse((int)HttpStatusCode.OK, true, "Update category successfully");
        }
        return new ApiResponse((int)HttpStatusCode.BadRequest, false, "Update category failed");
    }

    public async Task<ApiResponse<string>> DeleteCategoryAsync(int id)
    {
        var category = await _repoStore.Categories.FindAsync(id);
        if (category is null)
            return new ApiResponse<string>((int)HttpStatusCode.NotFound, false, $"Cannot found category with id: {id}");

        _repoStore.Categories.Remove(category);
        var result = await _repoStore.SaveChangesAsync();
        if (result)
        {
            await _cacheService.RemoveAsync("Categories");

            CategoryVM categoryvm = CreateCategoryVM(category);
            return new ApiResponse<string>((int)HttpStatusCode.OK, true, "Delete category successfully", categoryvm.Name);
        }
        return new ApiResponse<string>((int)HttpStatusCode.BadRequest, false, "Delete category failed", null!);
    }
}
