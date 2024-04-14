using API._Repositories;
using API._Services.Interfaces.UserManager;
using API.Helpers.Base;
using API.Helpers.Utilities;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ViewModels.Forum;
using ViewModels.System;

namespace API._Services.Services.UserManager;
public class S_User(IRepositoryAccessor repoStore, UserManager<User> userManager, RoleManager<IdentityRole> rolesManager) : BaseServices(repoStore), I_User
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _rolesManager = rolesManager;

    public async Task<OperationResult<User>> GetByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return OperationResult<User>.NotFound("User not found.");
        return OperationResult<User>.Success(user, "Get User Successfully.");
    }

    public async Task<OperationResult<PagingResult<ForumQuickVM>>> GetForumByUserId(string userId, PaginationParam pagination)
    {
        var query = from k in _repoStore.Forums.FindAll(true)
                    join c in _repoStore.Categories.FindAll(true) on k.CategoryId equals c.Id
                    where k.OwnerUserId == userId
                    orderby k.CreatedDate descending
                    select new { k, c };

        var totalRecords = await query.Select(u => new ForumQuickVM()
        {
            Id = u.k.Id,
            CategoryId = u.k.CategoryId,
            Description = u.k.Description,
            SeoAlias = u.k.SeoAlias,
            Title = u.k.Title,
            CategoryAlias = u.c.SeoAlias,
            CategoryName = u.c.Name,
            NumberOfVotes = u.k.NumberOfVotes,
            CreatedDate = u.k.CreatedDate
        }).ToListAsync();
        var result = PagingResult<ForumQuickVM>.Create(totalRecords, pagination.PageNumber, pagination.PageSize);
        return OperationResult<PagingResult<ForumQuickVM>>.Success(result, "Get forums by user id successfully.");
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
        return OperationResult<List<FunctionVM>>.Success(data, "Get data successfully.");
    }

}
