using API._Repositories;
using API._Services.Interfaces.System;
using API.Helpers.Base;
using API.Helpers.Utilities;
using API.Models;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;

namespace API._Services.Services.System;
public class S_Category(IRepositoryAccessor repoStore) : BaseServices(repoStore), I_Category
{
    public async Task<OperationResult<string>> CreateAsync(CategoryCreateRequest request)
    {
        Category? category = new()
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
            return OperationResult<string>.Success(category.Name, "Create category successfully");
        else
            return OperationResult<string>.BadRequest("Create category failed");

    }

    public async Task<OperationResult<PagingResult<CategoryVM>>> GetPagingAsync(string? filter, PaginationParam pagination, CategoryVM categoryVM)
    {
        IQueryable<Category>? query = _repoStore.Categories.FindAll(true);
        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x => x.SeoDescription.Contains(filter) || x.Name.Contains(filter));
        }
        var listCategoryVM = await query.Select(x => CreateCategoryVM(x)).ToListAsync();
        var resultPaging = PagingResult<CategoryVM>.Create(listCategoryVM, pagination.PageNumber, pagination.PageSize);
        return OperationResult<PagingResult<CategoryVM>>.Success(resultPaging, "Get function successfully.");
    }

    public async Task<OperationResult<CategoryVM>> FindByIdAsync(int id)
    {
        Category? category = await _repoStore.Categories.FindAsync(id);
        if (category is null)
            return OperationResult<CategoryVM>.NotFound($"Cannot found category with id: {id}");

        CategoryVM categoryVM = CreateCategoryVM(category);

        return OperationResult<CategoryVM>.Success(categoryVM, $"Get category by id {id} successfully.");
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

    public async Task<OperationResult> PutAsync(int id, CategoryCreateRequest request)
    {
        Category? category = await _repoStore.Categories.FindAsync(id);
        if (category is null)
            return OperationResult.NotFound($"Cannot found category with id: {id}");

        if (id == request.ParentId)
            return OperationResult.BadRequest("Category cannot be a child itself.");

        category.Name = request.Name;
        category.ParentId = request.ParentId;
        category.SortOrder = request.SortOrder;
        category.SeoDescription = request.SeoDescription;
        category.SeoAlias = request.SeoAlias;

        _repoStore.Categories.Update(category);
        bool result = await _repoStore.SaveChangesAsync();

        if (result)
            return OperationResult.Success("Update category successfully");
        return OperationResult.BadRequest("Update category failed");
    }

    public async Task<OperationResult<string>> DeleteAsync(int id)
    {
        Category? category = await _repoStore.Categories.FindAsync(id);
        if (category is null)
            return OperationResult<string>.NotFound($"Cannot found category with id: {id}");

        _repoStore.Categories.Remove(category);
        bool result = await _repoStore.SaveChangesAsync();
        if (result)
        {
            CategoryVM categoryvm = CreateCategoryVM(category);
            return OperationResult<string>.Success(categoryvm.Name, "Delete category successfully");
        }
        return OperationResult<string>.BadRequest("Delete category failed");
    }
}
