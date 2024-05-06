using API.Models;
using Microsoft.AspNetCore.Identity;
using static API.Helpers.Constants.SystemConstants;
namespace API.Data;
public class DbInitializer(DataContext context,
  UserManager<User> userManager,
  RoleManager<IdentityRole> roleManager)
{
    private readonly DataContext _context = context;
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly string AdminRoleName = "Admin";
    private readonly string UserRoleName = "Member";

    public async Task Seed()
    {
        #region Quyền

        if (!_roleManager.Roles.Any())
        {
            await _roleManager.CreateAsync(new IdentityRole
            {
                Id = AdminRoleName,
                Name = AdminRoleName,
                NormalizedName = AdminRoleName.ToUpper(),
            });
            await _roleManager.CreateAsync(new IdentityRole
            {
                Id = UserRoleName,
                Name = UserRoleName,
                NormalizedName = UserRoleName.ToUpper(),
            });
        }

        #endregion Quyền

        #region Người dùng

        if (!_userManager.Users.Any())
        {
            var result = await _userManager.CreateAsync(new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin",
                FullName = "Quản trị",
                Gender = Gender.Male,
                IsActive = true,
                DateOfBirth = new DateTime(1997, 01, 23),
                PhoneNumber = "0338716085",
                Email = "khanhtungmtp@gmail.com",
                LockoutEnabled = false
            }, "@@Tung23");
            if (result.Succeeded)
            {
                User? user = await _userManager.FindByNameAsync("admin");
                if (user != null)
                    await _userManager.AddToRoleAsync(user, AdminRoleName);
            }
        }

        #endregion Người dùng

        #region Chức năng

        if (!_context.Functions.Any())
        {
            _context.Functions.AddRange(new List<Function>
                {
                    new() {Id = "DASHBOARD", Name = "DASHBOARD", ParentId = "ROOT", SortOrder = 0,Url = "/admin/dashboard",Icon="dashboard"  },

                    new() {Id = "CONTENT",Name = "Nội dung",ParentId = "ROOT",SortOrder = 1,Url = "/admin/content",Icon="container" },

                    new() {Id = "CONTENT_CATEGORY",Name = "Danh mục",ParentId ="CONTENT",SortOrder = 2,Url = "/admin/content/category",Icon="folder"  },
                    new() {Id = "CONTENT_Forum",Name = "Bài viết",ParentId = "CONTENT",SortOrder = 3,Url = "/admin/content/kb",Icon="form" },
                    new() {Id = "CONTENT_COMMENT",Name = "Trang",ParentId = "CONTENT",SortOrder = 4,Url = "/admin/content/comment",Icon="message" },
                    new() {Id = "CONTENT_REPORT",Name = "Báo xấu",ParentId = "CONTENT",SortOrder = 5,Url = "/admin/content/report",Icon="line-chart" },

                    new() {Id = "STATISTIC",Name = "Thống kê", ParentId = "ROOT", SortOrder = 2,Url = "/statistic", Icon="pie-chart" },

                    new() {Id = "STATISTIC_MONTHLY_NEWMEMBER",Name = "Đăng ký từng tháng",ParentId = "STATISTIC",SortOrder = 1,Url = "/admin/statistic/monthly-register",Icon="bell"},
                    new() {Id = "STATISTIC_MONTHLY_NEWKB",Name = "Bài đăng hàng tháng",ParentId = "STATISTIC",SortOrder = 2,Url = "/admin/statistic/monthly-newkb",Icon="read"},
                    new() {Id = "STATISTIC_MONTHLY_COMMENT",Name = "Comment theo tháng",ParentId = "STATISTIC",SortOrder = 3,Url = "/admin/statistic/monthly-comment", Icon="message" },

                    new() {Id = "SYSTEM", Name = "Hệ thống", ParentId = "ROOT", SortOrder = 3,Url = "/system", Icon="setting" },

                    new() {Id = "SYSTEM_USER", Name = "Người dùng",ParentId = "SYSTEM",Url = "/admin/system/user-manager", Icon="usergroup-add"},
                    new() {Id = "SYSTEM_ROLE", Name = "Nhóm quyền",ParentId = "SYSTEM",Url = "/admin/system/role", Icon="team"},
                    new() {Id = "SYSTEM_FUNCTION", Name = "Chức năng",ParentId = "SYSTEM",Url = "/admin/system/function", Icon="folder-open"},
                    new() {Id = "SYSTEM_PERMISSION", Name = "Quyền hạn",ParentId = "SYSTEM",Url = "/admin/system/permission", Icon="unlock"},
                });
            await _context.SaveChangesAsync();
        }

        if (!_context.Commands.Any())
        {
            _context.Commands.AddRange(new List<Command>()
                {
                    new(){Id = "VIEW", Name = "Xem"},
                    new(){Id = "CREATE", Name = "Thêm"},
                    new(){Id = "UPDATE", Name = "Sửa"},
                    new(){Id = "DELETE", Name = "Xoá"},
                    new(){Id = "APPROVE", Name = "Duyệt"},
                });
        }

        #endregion Chức năng

        var functions = _context.Functions;

        if (!_context.CommandInFunctions.Any())
        {
            foreach (var function in functions)
            {
                var createAction = new CommandInFunction()
                {
                    CommandId = "CREATE",
                    FunctionId = function.Id
                };
                _context.CommandInFunctions.Add(createAction);

                var updateAction = new CommandInFunction()
                {
                    CommandId = "UPDATE",
                    FunctionId = function.Id
                };
                _context.CommandInFunctions.Add(updateAction);
                var deleteAction = new CommandInFunction()
                {
                    CommandId = "DELETE",
                    FunctionId = function.Id
                };
                _context.CommandInFunctions.Add(deleteAction);

                var viewAction = new CommandInFunction()
                {
                    CommandId = "VIEW",
                    FunctionId = function.Id
                };
                _context.CommandInFunctions.Add(viewAction);
            }
        }

        if (!_context.Permissions.Any())
        {
            var adminRole = await _roleManager.FindByNameAsync(AdminRoleName);
            if (functions != null && adminRole != null)
            {
                foreach (var function in functions)
                {
                    _context.Permissions.Add(new Permission(function.Id, adminRole.Id, "CREATE"));
                    _context.Permissions.Add(new Permission(function.Id, adminRole.Id, "UPDATE"));
                    _context.Permissions.Add(new Permission(function.Id, adminRole.Id, "DELETE"));
                    _context.Permissions.Add(new Permission(function.Id, adminRole.Id, "VIEW"));
                }
            }
        }

        // Checking if there are any languages already seeded
        if (!_context.SystemLanguages.Any())
        {
            var languages = new List<SystemLanguage>
            {
                new()
                {
                    Id = "en_US",
                    Name = "English",
                    UrlImage = "en.png",
                    IsActive = true
                },
                new()
                {
                    Id = "vi_VN",
                    Name = "Vietnamese",
                    UrlImage = "vn.png",
                    IsActive = true
                },
                new()
                {
                    Id = "zh_TW",
                    Name = "繁體中文",
                    UrlImage = "zh.png",
                    IsActive = true
                }
            };

            await _context.SystemLanguages.AddRangeAsync(languages);
            await _context.SaveChangesAsync();
        }

        await _context.SaveChangesAsync();
    }
}
