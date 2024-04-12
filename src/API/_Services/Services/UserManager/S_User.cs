using API._Repositories;
using API._Services.Interfaces.UserManager;
using API.Helpers.Base;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;

namespace API._Services.Services.UserManager;
public class S_User(IRepositoryAccessor repoStore, UserManager<User> userManager, RoleManager<IdentityRole> rolesManager) : BaseServices(repoStore), I_User
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _rolesManager = rolesManager;

    public async Task<User?> GetById(string id)
    {
        return await _repoStore.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<OperationResult<List<FunctionVM>>> GetMenuByUserPermission(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return OperationResult<List<FunctionVM>>.NotFound("User not found.");
        var roles = await _userManager.GetRolesAsync(user);
        IQueryable<FunctionVM>? query = from f in _repoStore.Functions.FindAll(true)
                                        join p in _repoStore.Permissions.FindAll(true)
                                            on f.Id equals p.FunctionId
                                        join r in _rolesManager.Roles on p.RoleId equals r.Id
                                        join a in _repoStore.Commands.FindAll(true)
                                            on p.CommandId equals a.Id
                                        where roles.Contains(r.Name ?? string.Empty) && a.Id == "VIEW"
                                        select new FunctionVM
                                        {
                                            Id = f.Id,
                                            Name = f.Name,
                                            Url = f.Url,
                                            ParentId = f.ParentId,
                                            SortOrder = f.SortOrder,
                                            Icon = f.Icon
                                        };
        var data = await query.Distinct()
            .OrderBy(x => x.ParentId)
            .ThenBy(x => x.SortOrder)
            .ToListAsync();
        return OperationResult<List<FunctionVM>>.Success(data, "Function updated successfully.");
    }
}
