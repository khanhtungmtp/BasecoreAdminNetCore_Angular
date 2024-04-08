using System.Net;
using API._Repositories;
using API._Services.Interfaces.System;
using API.Helpers.Base;
using API.Models;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;

namespace API._Services.Services.System;
public class S_CommandInFunction(IRepositoryAccessor repoStore) : BaseServices(repoStore), I_CommandInFunction
{
    public async Task<ApiResponse<List<CommandVM>>> FindIdsCommandInFunctionAsync(string functionId)
    {
        var query = from a in _repoStore.Commands.FindAll(true)
                    join commandinfunc in _repoStore.CommandInFunctions.FindAll(true) on a.Id equals commandinfunc.CommandId into result1
                    from commandInFunction in result1.DefaultIfEmpty()
                    join func in _repoStore.Functions.FindAll(true) on commandInFunction.FunctionId equals func.Id into result2
                    from function in result2.DefaultIfEmpty()
                    select new
                    {
                        a.Id,
                        a.Name,
                        commandInFunction.FunctionId
                    };

        query = query.Where(x => x.FunctionId == functionId);

        List<CommandVM>? data = await query.Select(x => new CommandVM()
        {
            Id = x.Id,
            Name = x.Name
        }).ToListAsync();
        return Success((int)HttpStatusCode.OK, data, "Get command in function successfully.");
    }

    // PostCommandInFunction
    public async Task<ApiResponse<CommandInFunctionResponseVM>> CreateAsync(string functionId, CommandAssignRequest request)
    {
        foreach (var commandId in request.CommandIds)
        {
            if (await _repoStore.CommandInFunctions.FindAsync(commandId, functionId) != null)
                return Fail<CommandInFunctionResponseVM>((int)HttpStatusCode.Conflict, "Command already exists in function.");

            var entity = new CommandInFunction()
            {
                CommandId = commandId,
                FunctionId = functionId
            };

            _repoStore.CommandInFunctions.Add(entity);
        }

        if (request.AddToAllFunctions)
        {
            IQueryable<Function>? otherFunctions = _repoStore.Functions.FindAll(x => x.Id != functionId);
            foreach (var function in otherFunctions)
            {
                foreach (string? commandId in request.CommandIds)
                {
                    if (await _repoStore.CommandInFunctions.FindAsync(request.CommandIds, function.Id) is null)
                    {
                        _repoStore.CommandInFunctions.Add(new CommandInFunction()
                        {
                            CommandId = commandId,
                            FunctionId = function.Id
                        });
                    }
                }
            }
        }
        bool result = await _repoStore.SaveChangesAsync();

        if (result)
            return Success((int)HttpStatusCode.OK, new CommandInFunctionResponseVM() { CommandIds = request.CommandIds, FunctionId = functionId }, "Add command to function successfully.");
        else
            return Fail<CommandInFunctionResponseVM>((int)HttpStatusCode.InternalServerError, "Add command to function failed.");
    }

    public async Task<ApiResponse> DeleteAsync(string functionId, CommandAssignRequest request)
    {
        foreach (var commandId in request.CommandIds)
        {
            var entity = await _repoStore.CommandInFunctions.FindAsync(commandId, functionId);
            if (entity is null)
                return Fail((int)HttpStatusCode.NotFound, "This command is not existed in function");

            _repoStore.CommandInFunctions.Remove(entity);
        }

        bool result = await _repoStore.SaveChangesAsync();

        if (result)
            return Success((int)HttpStatusCode.OK, "Command to function delete successfully.");
        else
            return Fail((int)HttpStatusCode.OK, "Delete command to function failed.");

    }
}
