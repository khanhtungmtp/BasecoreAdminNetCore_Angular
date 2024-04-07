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
    public Task<ApiResponse<string>> CreateAsync(FunctionCreateRequest request)
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
        _repoStore.SaveChangesAsync();
        return Task.FromResult(new ApiResponse<string>((int)HttpStatusCode.OK, true, "Function created successfully.", function.Id));
    }

    public async Task<ApiResponse<FunctionVM>> FindByIdAsync(string id)
    {
        Function? function = await _repoStore.Functions.FindByIdAsync(id);
        if (function is null)
            return new ApiResponse<FunctionVM>((int)HttpStatusCode.NotFound, false, "Function not found.", null!);
        FunctionVM userVM = new()
        {
            Id = function.Id,
            Name = function.Name,
            Url = function.Url,
        };
        return new ApiResponse<FunctionVM>((int)HttpStatusCode.OK, true, "Get function successfully.", userVM);
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
        return new ApiResponse<PagingResult<FunctionVM>>((int)HttpStatusCode.OK, true, "Get function successfully.", resultPaging);
    }

    public async Task<ApiResponse<string>> PutFunctionAsync(string id, FunctionCreateRequest request)
    {
        Function? function = await _repoStore.Functions.FindByIdAsync(id);
        if (function is null || function.Id != request.Id)
            return new ApiResponse<string>((int)HttpStatusCode.NotFound, false, "Function not found.", null!);
        function.Name = request.Name;
        function.ParentId = request.ParentId;
        function.SortOrder = request.SortOrder;
        function.Url = request.Url;
        function.Icon = request.Icon;
        _repoStore.Functions.Update(function);
        var result = await _repoStore.SaveChangesAsync();
        if (result)
            return new ApiResponse<string>((int)HttpStatusCode.OK, true, "Function updated successfully.", function.Id);

        return new ApiResponse<string>(500, false, "Function update failed.", null!);
    }

    public async Task<ApiResponse<string>> DeleteFunctionAsync(string id)
    {
        Function? function = await _repoStore.Functions.FindByIdAsync(id);
        if (function is null)
            return new ApiResponse<string>((int)HttpStatusCode.NotFound, false, "Function not found.", null!);

        _repoStore.Functions.Remove(function);
        // remove command in function
        List<CommandInFunction>? commands = await _repoStore.CommandInFunctions.FindAll(x => x.FunctionId == id).ToListAsync();
        // if (commands.Count > 0)
        _repoStore.CommandInFunctions.RemoveMultiple(commands);

        var result = await _repoStore.SaveChangesAsync();
        if (result)
            return new ApiResponse<string>((int)HttpStatusCode.OK, true, "Function deleted successfully.", function.Id);
        return new ApiResponse<string>(500, false, "Function delete failed.", null!);
    }
}
