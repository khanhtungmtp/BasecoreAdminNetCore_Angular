using API._Repositories;
using API._Services.Interfaces.System;
using API.Helpers.Base;
using API.Helpers.Utilities;
using API.Models;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;

namespace API._Services.Services.System;

public class S_Function(IRepositoryAccessor repoStore) : BaseServices(repoStore), I_Function
{
    public async Task<OperationResult<string>> CreateAsync(FunctionCreateRequest request)
    {
        Function? functionExists = await _repoStore.Functions.FindByIdAsync(request.Id);
        if (functionExists is not null)
            return OperationResult<string>.Conflict("Function is existed.");
        var function = new Function()
        {
            Id = request.Id,
            Name = request.Name,
            ParentId = request.ParentId,
            SortOrder = request.SortOrder,
            Url = request.Url,
            Icon = request.Icon
        };

        await _repoStore.Functions.AddAsync(function);
        await _repoStore.SaveChangesAsync();
        return OperationResult<string>.Success(function.Id, "Function created successfully.");
    }

    public async Task<OperationResult<FunctionVM>> FindByIdAsync(string id)
    {
        Function? function = await _repoStore.Functions.FindByIdAsync(id);
        if (function is null)
            return OperationResult<FunctionVM>.NotFound("Function not found.");
        FunctionVM userVM = new()
        {
            Id = function.Id,
            Name = function.Name,
            Url = function.Url,
            Icon = function.Icon,
            ParentId = function.ParentId,
            SortOrder = function.SortOrder
        };
        return OperationResult<FunctionVM>.Success(userVM, "Get function by id successfully.");
    }

    public async Task<OperationResult<PagingResult<FunctionVM>>> GetPagingAsync(string? filter, PaginationParam pagination, FunctionVM userVM)
    {
        var query = _repoStore.Functions.FindAll(true);
        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x => x.Id.Contains(filter) || x.Name.Contains(filter));
        }
        var listFunctionVM = await query.Select(x => new FunctionVM()
        {
            Id = x.Id,
            Name = x.Name,
            Url = x.Url,
            ParentId = x.ParentId,
            SortOrder = x.SortOrder,
            Icon = x.Icon
        }).ToListAsync();
        var resultPaging = PagingResult<FunctionVM>.Create(listFunctionVM, pagination.PageNumber, pagination.PageSize);
        return OperationResult<PagingResult<FunctionVM>>.Success(resultPaging, "Get function successfully.");
    }

    public async Task<OperationResult<string>> PutAsync(string id, FunctionCreateRequest request)
    {
        Function? function = await _repoStore.Functions.FindByIdAsync(id);
        if (function is null || function.Id != request.Id)
            return OperationResult<string>.NotFound("Function not found.");
        function.Name = request.Name;
        function.ParentId = request.ParentId;
        function.SortOrder = request.SortOrder;
        function.Url = request.Url;
        function.Icon = request.Icon;
        _repoStore.Functions.Update(function);
        bool result = await _repoStore.SaveChangesAsync();
        if (result)
            return OperationResult<string>.Success(function.Id, "Function updated successfully.");

        return OperationResult<string>.BadRequest("Function update failed.");
    }

    public async Task<OperationResult<string>> DeleteAsync(string id)
    {
        Function? function = await _repoStore.Functions.FindByIdAsync(id);
        if (function is null)
            return OperationResult<string>.NotFound("Function not found.");

        _repoStore.Functions.Remove(function);
        // remove command in function
        List<CommandInFunction>? commands = await _repoStore.CommandInFunctions.FindAll(x => x.FunctionId == id).ToListAsync();
        // if (commands.Count > 0)
        _repoStore.CommandInFunctions.RemoveMultiple(commands);

        bool result = await _repoStore.SaveChangesAsync();
        if (result)
            return OperationResult<string>.Success(function.Id, "Function deleted successfully.");
        return OperationResult<string>.BadRequest("Function delete failed.");
    }

    public async Task<OperationResult<List<KeyValuePair<string, string>>>> GetParentIdsAsync()
    {
        var data = await _repoStore.Functions.FindAll(true).Select(x => new KeyValuePair<string, string>(x.Id, x.Name)).ToListAsync();
        return OperationResult<List<KeyValuePair<string, string>>>.Success(data, "Get functions successfully.");
    }
}
