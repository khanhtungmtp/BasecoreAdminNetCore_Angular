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
        await _repoStore.Categories.AddAsync(category);
        bool result = await _repoStore.SaveChangesAsync();

        if (result)
        {
            await _cacheService.RemoveAsync("Categories");

            return Success((int)HttpStatusCode.OK, "Create category successfully", category.Id.ToString());
        }
        else
            return Fail<string>((int)HttpStatusCode.BadRequest, "Create category failed");

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
        return Success((int)HttpStatusCode.OK, resultPaging, "Get function successfully.");
    }

    public async Task<ApiResponse<CategoryVM>> FindByIdAsync(int id)
    {
        var category = await _repoStore.Categories.FindAsync(id);
        if (category is null)
            return Fail<CategoryVM>((int)HttpStatusCode.NotFound, $"Cannot found category with id: {id}");

        CategoryVM categoryVM = CreateCategoryVM(category);

        return Success((int)HttpStatusCode.OK, categoryVM, $"Get category by id {id} successfully.");
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
            return Fail((int)HttpStatusCode.NotFound, $"Cannot found category with id: {id}");

        if (id == request.ParentId)
            return Fail((int)HttpStatusCode.BadRequest, "Category cannot be a child itself.");

        category.Name = request.Name;
        category.ParentId = request.ParentId;
        category.SortOrder = request.SortOrder;
        category.SeoDescription = request.SeoDescription;
        category.SeoAlias = request.SeoAlias;

        _repoStore.Categories.Update(category);
        bool result = await _repoStore.SaveChangesAsync();

        if (result)
        {
            await _cacheService.RemoveAsync("Categories");

            return Success((int)HttpStatusCode.OK, "Update category successfully");
        }
        return Fail((int)HttpStatusCode.BadRequest, "Update category failed");
    }

    public async Task<ApiResponse<string>> DeleteCategoryAsync(int id)
    {
        var category = await _repoStore.Categories.FindAsync(id);
        if (category is null)
            return Fail<string>((int)HttpStatusCode.NotFound, $"Cannot found category with id: {id}");

        _repoStore.Categories.Remove(category);
        bool result = await _repoStore.SaveChangesAsync();
        if (result)
        {
            await _cacheService.RemoveAsync("Categories");

            CategoryVM categoryvm = CreateCategoryVM(category);
            return Success((int)HttpStatusCode.OK, categoryvm.Name, "Delete category successfully");
        }
        return Fail<string>((int)HttpStatusCode.BadRequest, "Delete category failed");
    }
}
