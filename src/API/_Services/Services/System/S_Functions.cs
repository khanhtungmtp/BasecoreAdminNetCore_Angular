using API._Repositories;
using API._Services.Interfaces.System;
using API.Helpers.Base;
using API.Models;
using ViewModels.System;

namespace API._Services.Services.System;

public class S_Function(IRepositoryAccessor repositoryAccessor) : BaseServices(repositoryAccessor), I_Function
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

        _repositoryAccessor.Functions.Add(function);
        _repositoryAccessor.SaveChangesAsync();
        return Task.FromResult(new ApiResponse<string>(true, "Function created successfully.", function.Id));
    }

    public async Task<ApiResponse<FunctionVM>> FindByIdAsync(string id)
    {
        var function = await _repositoryAccessor.Functions.FindByIdAsync(id);
        if (function is null)
            return new ApiResponse<FunctionVM>(false, "Function not found.", null!);
        var userVM = new FunctionVM()
        {
            Id = function.Id,
            Name = function.Name,
            Url = function.Url,
        };
        return new ApiResponse<FunctionVM>(true, "", userVM);
    }
}
