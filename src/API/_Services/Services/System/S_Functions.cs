using System.Net;
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
    public async Task<ApiResponse<string>> CreateAsync(FunctionCreateRequest request)
    {
        var function = new Function()
        {
            Id = request.Id,
            Name = request.Name,
            ParentId = request.ParentId,
            SortOrder = request.SortOrder,
            Url = request.Url,
            Icon = request.Icon
        };

        _repoStore.Functions.Add(function);
        await _repoStore.SaveChangesAsync();
        return Success((int)HttpStatusCode.OK, function.Id, "Function created successfully.");
    }

    public async Task<ApiResponse<FunctionVM>> FindByIdAsync(string id)
    {
        Function? function = await _repoStore.Functions.FindByIdAsync(id);
        if (function is null)
            return Fail<FunctionVM>((int)HttpStatusCode.NotFound, "Function not found.");
        FunctionVM userVM = new()
        {
            Id = function.Id,
            Name = function.Name,
            Url = function.Url,
        };
        return Success((int)HttpStatusCode.OK, userVM, "Get function successfully.");
    }

    public async Task<ApiResponse<PagingResult<FunctionVM>>> GetAllPaging(string? filter, PaginationParam pagination, FunctionVM userVM)
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
            Url = x.Url
        }).ToListAsync();
        var resultPaging = PagingResult<FunctionVM>.Create(listFunctionVM, pagination.PageNumber, pagination.PageSize);
        return Success((int)HttpStatusCode.OK, resultPaging, "Get function successfully.");
    }

    public async Task<ApiResponse<string>> PutFunctionAsync(string id, FunctionCreateRequest request)
    {
        Function? function = await _repoStore.Functions.FindByIdAsync(id);
        if (function is null || function.Id != request.Id)
            return Fail<string>((int)HttpStatusCode.NotFound, "Function not found.");
        function.Name = request.Name;
        function.ParentId = request.ParentId;
        function.SortOrder = request.SortOrder;
        function.Url = request.Url;
        function.Icon = request.Icon;
        _repoStore.Functions.Update(function);
        bool result = await _repoStore.SaveChangesAsync();
        if (result)
            return Success((int)HttpStatusCode.OK, function.Id, "Function updated successfully.");

        return Fail<string>((int)HttpStatusCode.InternalServerError, "Function update failed.");
    }

    public async Task<ApiResponse<string>> DeleteFunctionAsync(string id)
    {
        Function? function = await _repoStore.Functions.FindByIdAsync(id);
        if (function is null)
            return Fail<string>((int)HttpStatusCode.NotFound, "Function not found.");

        _repoStore.Functions.Remove(function);
        // remove command in function
        List<CommandInFunction>? commands = await _repoStore.CommandInFunctions.FindAll(x => x.FunctionId == id).ToListAsync();
        // if (commands.Count > 0)
        _repoStore.CommandInFunctions.RemoveMultiple(commands);

        bool result = await _repoStore.SaveChangesAsync();
        if (result)
            return Success((int)HttpStatusCode.OK, function.Id, "Function deleted successfully.");
        return Fail<string>((int)HttpStatusCode.InternalServerError, "Function delete failed.");
    }
}
