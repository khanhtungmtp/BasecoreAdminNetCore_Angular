using API._Repositories;
using API._Services.Interfaces.UserManager;
using API.Helpers.Base;
using API.Models;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;

namespace API._Services.Services.UserManager;
public class S_Roles(IRepositoryAccessor repoStore) : BaseServices(repoStore), I_Roles
{
    public async Task<OperationResult<List<PermissionVm>>> GetPermissionByRoleId(string roleId)
    {
        List<PermissionVm>? permissions = await (from p in _repoStore.Permissions.FindAll(true)
                                                 join a in _repoStore.Commands.FindAll(true)
                                                 on p.CommandId equals a.Id
                                                 where p.RoleId == roleId
                                                 select new PermissionVm()
                                                 {
                                                     FunctionId = p.FunctionId,
                                                     CommandId = p.CommandId,
                                                     RoleId = p.RoleId
                                                 }).ToListAsync();
        return OperationResult<List<PermissionVm>>.Success(permissions, "Get permission by role id successfully.");
    }

    public async Task<OperationResult<string>> PutPermissionByRoleId(string roleId, UpdatePermissionRequest request)
    {
        //create new permission list from user changed
        var newPermissions = new List<Permission>();
        foreach (var p in request.Permissions)
        {
            newPermissions.Add(new Permission(p.FunctionId, roleId, p.CommandId));
        }
        var existingPermissions = _repoStore.Permissions.FindAll(x => x.RoleId == roleId);

        _repoStore.Permissions.RemoveMany(existingPermissions);
        _repoStore.Permissions.AddMany(newPermissions.Distinct(new MyPermissionComparer()));
        bool result = await _repoStore.SaveChangesAsync();
        if (result)
            return OperationResult<string>.Success("Save permission successfully");

        return OperationResult<string>.BadRequest("Save permission failed");
    }

    internal class MyPermissionComparer : IEqualityComparer<Permission>
    {
        // Items are equal if their ids are equal.
        public bool Equals(Permission? x, Permission? y)
        {
            // Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            // Check whether any of the compared objects is null.
            if (x is null || y is null)
                return false;

            // Check whether the items' properties are equal.
            return x.CommandId == y.CommandId && x.FunctionId == y.FunctionId && x.RoleId == y.RoleId;
        }

        public int GetHashCode(Permission permission)
        {
            // Check whether the object is null
            if (permission is null) return 0;

            // Use HashCode.Combine to generate a hash code considering all the properties.
            return HashCode.Combine(permission.CommandId, permission.FunctionId, permission.RoleId);
        }
    }
}
