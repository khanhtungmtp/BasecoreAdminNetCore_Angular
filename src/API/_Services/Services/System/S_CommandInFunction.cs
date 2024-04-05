using System.Net;
using API._Repositories;
using API._Services.Interfaces.System;
using API.Helpers.Base;
using API.Models;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;

namespace API._Services.Services.System;
public class S_CommandInFunction(IRepositoryAccessor repositoryAccessor) : BaseServices(repositoryAccessor), I_CommandInFunction
{
    public async Task<ApiResponse<List<CommandVm>>> GetCommandInFunction(string functionId)
    {
        var query = from a in _repositoryAccessor.Commands.FindAll(true)
                    join commandinfunc in _repositoryAccessor.CommandInFunctions.FindAll(true) on a.Id equals commandinfunc.CommandId into result1
                    from commandInFunction in result1.DefaultIfEmpty()
                    join func in _repositoryAccessor.Functions.FindAll(true) on commandInFunction.FunctionId equals func.Id into result2
                    from function in result2.DefaultIfEmpty()
                    select new
                    {
                        a.Id,
                        a.Name,
                        commandInFunction.FunctionId
                    };

        query = query.Where(x => x.FunctionId == functionId);

        List<CommandVm>? data = await query.Select(x => new CommandVm()
        {
            Id = x.Id,
            Name = x.Name
        }).ToListAsync();
        return new ApiResponse<List<CommandVm>>((int)HttpStatusCode.OK, true, data);
    }

    // PostCommandInFunction
    public async Task<ApiResponse<object>> PostCommandInFunction(string functionId, CommandAssignRequest request)
    {
        foreach (var commandId in request.CommandIds)
        {
            if (await _repositoryAccessor.CommandInFunctions.FindAsync(commandId, functionId) != null)
                return new ApiResponse<object>((int)HttpStatusCode.Conflict, false, "Command already exists in function.", null!);

            var entity = new CommandInFunction()
            {
                CommandId = commandId,
                FunctionId = functionId
            };

            _repositoryAccessor.CommandInFunctions.Add(entity);
        }

        if (request.AddToAllFunctions)
        {
            IQueryable<Function>? otherFunctions = _repositoryAccessor.Functions.FindAll(x => x.Id != functionId);
            foreach (var function in otherFunctions)
            {
                foreach (string? commandId in request.CommandIds)
                {
                    if (await _repositoryAccessor.CommandInFunctions.FindAsync(request.CommandIds, function.Id) == null)
                    {
                        _repositoryAccessor.CommandInFunctions.Add(new CommandInFunction()
                        {
                            CommandId = commandId,
                            FunctionId = function.Id
                        });
                    }
                }
            }
        }
        bool result = await _repositoryAccessor.SaveChangesAsync();

        if (result)
            return new ApiResponse<object>((int)HttpStatusCode.OK, true, new { request.CommandIds, functionId });
        else
            return new ApiResponse<object>(500, false, "Add command to function failed.", null!);
    }

    public async Task<ApiResponse> DeleteCommandInFunction(string functionId, CommandAssignRequest request)
    {
        foreach (var commandId in request.CommandIds)
        {
            var entity = await _repositoryAccessor.CommandInFunctions.FindAsync(commandId, functionId);
            if (entity is null)
                return new ApiResponse((int)HttpStatusCode.NotFound, "This command is not existed in function");

            _repositoryAccessor.CommandInFunctions.Remove(entity);
        }

        bool result = await _repositoryAccessor.SaveChangesAsync();

        if (result)
            return new ApiResponse((int)HttpStatusCode.OK, "Command to function delete successfully.");
        else
            return new ApiResponse((int)HttpStatusCode.OK, "Delete command to function failed.");

    }
}
