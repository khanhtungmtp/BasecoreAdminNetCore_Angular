

using System.Net;
using API._Repositories;
using API._Services.Interfaces.UserManager;
using API.Helpers.Base;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;

namespace API._Services.Services.UserManager;
public class S_User : BaseServices, I_User
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _rolesManager;
    public S_User(IRepositoryAccessor repositoryAccessor, UserManager<User> userManager, RoleManager<IdentityRole> rolesManager) : base(repositoryAccessor)
    {
        _userManager = userManager;
        _rolesManager = rolesManager;
    }

    public async Task<ApiResponse<List<FunctionVM>>> GetMenuByUserPermission(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new ApiResponse<List<FunctionVM>>((int)HttpStatusCode.NotFound, false, "User not found.", null!);
        var roles = await _userManager.GetRolesAsync(user);
        IQueryable<FunctionVM>? query = from f in _repositoryAccessor.Functions.FindAll(true)
                                        join p in _repositoryAccessor.Permissions.FindAll(true)
                                            on f.Id equals p.FunctionId
                                        join r in _rolesManager.Roles on p.RoleId equals r.Id
                                        join a in _repositoryAccessor.Commands.FindAll(true)
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
        return new ApiResponse<List<FunctionVM>>((int)HttpStatusCode.OK, true, data);
    }
}
