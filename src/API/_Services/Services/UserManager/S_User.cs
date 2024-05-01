using System.Linq.Expressions;
using API._Repositories;
using API._Services.Interfaces.UserManager;
using API.Helpers.Base;
using API.Helpers.Constants;
using API.Helpers.Utilities;
using API.Models;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ViewModels.Forum;
using ViewModels.System;
using ViewModels.UserManager;

namespace API._Services.Services.UserManager;
public class S_User(IRepositoryAccessor repoStore, UserManager<User> userManager, RoleManager<IdentityRole> rolesManager) : BaseServices(repoStore), I_User
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _rolesManager = rolesManager;

    public async Task<OperationResult<UserVM>> GetByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return OperationResult<UserVM>.NotFound("User not found.");
        var userVM = new UserVM()
        {
            Id = user.Id,
            FullName = user.FullName,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            DateOfBirth = user.DateOfBirth,
            IsActive = user.IsActive,
            Gender = (Gender)user.Gender,
            Roles = [.. _userManager.GetRolesAsync(user).Result]
        };
        return OperationResult<UserVM>.Success(userVM, "Get User Successfully.");
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

    public async Task<OperationResult<PagingResult<UserVM>>> GetPaging(PaginationParam pagination, UserSearchRequest userSearchRequest)
    {
        // Tạo mệnh đề predicate
        var predicate = BuildUserSearchPredicate(userSearchRequest);

        // Thực hiện truy vấn
        var usersQuery = await _userManager.Users.AsNoTracking().Where(predicate).ToListAsync();

        // Tạo list UserVM từ pagedUsers và lấy roles cho mỗi user
        var listUserVM = new List<UserVM>();

        foreach (var user in usersQuery)
        {
            var roles = await _userManager.GetRolesAsync(user); // Lấy roles dựa trên user hiện tại

            var userVM = new UserVM
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                DateOfBirth = user.DateOfBirth,
                IsActive = user.IsActive,
                Gender = (Gender)user.Gender,
                Roles = [.. roles] // Lưu tên của roles vào UserVM
            };

            listUserVM.Add(userVM);
        }

        // Tạo đối tượng PagingResult<UserVM> từ listUserVM
        var resultPaging = PagingResult<UserVM>.Create(listUserVM, pagination.PageNumber, pagination.PageSize);

        // Trả về kết quả
        return OperationResult<PagingResult<UserVM>>.Success(resultPaging, "Get Users Successfully");
    }

    private Expression<Func<User, bool>> BuildUserSearchPredicate(UserSearchRequest userSearchRequest)
    {
        var predicate = PredicateBuilder.New<User>(true);

        if (!string.IsNullOrWhiteSpace(userSearchRequest.UserName))
        {
            predicate = predicate.And(u => u.UserName != null && u.UserName.Contains(userSearchRequest.UserName));
        }

        if (!string.IsNullOrWhiteSpace(userSearchRequest.Email))
        {
            predicate = predicate.And(u => u.Email != null && u.Email.Contains(userSearchRequest.Email));
        }

        if (!string.IsNullOrWhiteSpace(userSearchRequest.PhoneNumber))
        {
            predicate = predicate.And(u => u.PhoneNumber != null && u.PhoneNumber.Contains(userSearchRequest.PhoneNumber));
        }

        if (!string.IsNullOrWhiteSpace(userSearchRequest.FullName))
        {
            predicate = predicate.And(u => u.FullName.Contains(userSearchRequest.FullName));
        }

        if (userSearchRequest.Gender.HasValue)
        {
            predicate = predicate.And(u => u.Gender == (SystemConstants.Gender)userSearchRequest.Gender.Value);
        }

        if (userSearchRequest.IsActive.HasValue)
        {
            predicate = predicate.And(u => u.IsActive == userSearchRequest.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(userSearchRequest.DateOfBirth))
        {
            bool isDate = DateTime.TryParse(userSearchRequest.DateOfBirth, out DateTime filterDate);
            if (isDate)
            {
                predicate = predicate.And(u => u.DateOfBirth.Date == filterDate.Date);
            }
        }

        return predicate;
    }

    public async Task<OperationResult<List<FunctionTreeVM>>> GetMenuByUserPermission(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return OperationResult<List<FunctionTreeVM>>.NotFound("User not found.");
        var roles = await _userManager.GetRolesAsync(user);
        IQueryable<FunctionTreeVM>? query = from f in _repoStore.Functions.FindAll(true)
                                        join p in _repoStore.Permissions.FindAll(true)
                                            on f.Id equals p.FunctionId
                                        join r in _rolesManager.Roles on p.RoleId equals r.Id
                                        join a in _repoStore.Commands.FindAll(true)
                                            on p.CommandId equals a.Id
                                        where roles.Contains(r.Name ?? string.Empty) && a.Id == "VIEW"
                                            select new FunctionTreeVM
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
        var dataTree = FunctionUtility.UnflatteringForLeftMenu(data);
        return OperationResult<List<FunctionTreeVM>>.Success(dataTree, "Get data successfully.");
    }

    public async Task<OperationResult> DeleteRangeAsync(List<string> ids, string idLogedIn)
    {
        if (ids.Count == 0) return OperationResult.NotFound("List users empty.");

        if (ids.Contains(idLogedIn))
            return OperationResult.BadRequest("Cannot delete the list of users. The list of ids contains the userid of the currently logged in user.");

        var entitiesToDelete = await _repoStore.Users.FindAll(entity => ids.Contains(entity.Id)).ToListAsync();

        if (entitiesToDelete.Count == 0)
            return OperationResult.NotFound("List users empty.");

        _repoStore.Users.RemoveMany(entitiesToDelete);
        await _repoStore.SaveChangesAsync();
        return OperationResult.Success("Delete users successfully.");
    }

}
