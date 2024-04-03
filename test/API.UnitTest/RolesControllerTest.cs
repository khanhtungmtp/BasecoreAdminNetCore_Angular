using API.Controllers;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ViewModels.UserManager;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using API.Helpers.Utilities;
using MockQueryable.Moq;
using Microsoft.EntityFrameworkCore;

namespace API.UnitTest
{
    public class RolesControllerTest
    {
        private readonly Mock<RoleManager<IdentityRole>> _roleManager;
        public RolesControllerTest()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _roleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object,
                It.IsAny<IEnumerable<IRoleValidator<IdentityRole>>>(),
                It.IsAny<ILookupNormalizer>(),
                It.IsAny<IdentityErrorDescriber>(),
                It.IsAny<ILogger<RoleManager<IdentityRole>>>());
        }

        [Fact] // test inject controller
        public void CreateInstance_NotNull()
        {
            var roleController = new RolesController(_roleManager.Object);
            Assert.NotNull(roleController);
        }

        [Fact] // create role Success
        public async Task CreateRole_ValidInput_Success()
        {
            // gia lap create thanh cong
            _roleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);
            //Act
            var roleController = new RolesController(_roleManager.Object);
            var result = await roleController.CreateRole(new RoleVM() { Id = "admin", Name = "Admin" });
            Assert.NotNull(result);
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact] // create role failed
        public async Task CreateRole_ValidInput_Failed()
        {
            // gia lap create that bai
            _roleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Failed(new IdentityError[] { }));
            var roleController = new RolesController(_roleManager.Object);
            var result = await roleController.CreateRole(new RoleVM() { Id = "admin", Name = "Admin" });
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact] // get allroles Success
        public async Task GetRolePaging_HasData_Success()
        {
            // Given
            var roles = new List<IdentityRole>(){
            new(){
                Id = "admin",
                Name = "Admin"
            },
            new(){
                Id = "admin2",
                Name = "Admin2"
            },
            new(){
                Id = "admin3",
                Name = "Admin3"
            },
            new(){
                Id = "user",
                Name = "User"
            }
        };
            _roleManager.Setup(x => x.Roles).Returns(roles.AsQueryable().BuildMock());
            var roleController = new RolesController(_roleManager.Object);
            var paginationParam = new PaginationParam();
            var roleVM = new RoleVM()
            {
                Id = "admin",
                Name = "Admin"
            };
            // Arrange
            var result = await roleController.GetAll(paginationParam, roleVM);
            var okResult = result as OkObjectResult;
            var roleVms = okResult?.Value as PaginationUtility<RoleVM>;
            Assert.Equal(3, roleVms?.Pagination.TotalCount);
        }

        [Fact] // get allroles failed
        public async Task GetRole_ThrowException_Failed()
        {
            // gia lap get loi
            _roleManager.Setup(x => x.Roles).Throws<Exception>();
            // Act
            var roleController = new RolesController(_roleManager.Object);
            var paginationParam = new PaginationParam();
            await Assert.ThrowsAnyAsync<Exception>(async () => await roleController.GetAll(paginationParam, new RoleVM() { }));
        }

        [Fact]
        public async Task GetById_HasData_Success()
        {
            // Arrange
            _roleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new IdentityRole()
            {
                Id = "admin",
                Name = "Admin"
            });
            // Act
            var roleController = new RolesController(_roleManager.Object);
            var result = await roleController.GetById("admin");
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            var roleVM = okResult?.Value as RoleVM;
            Assert.Equal("admin", roleVM?.Id);
        }

        [Fact]
        public async Task GetByID_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            var roleId = "non-existing-id";
            _roleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityRole?)null);

            // Act
            var roleController = new RolesController(_roleManager.Object);
            var result = await roleController.GetById(roleId);
            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

    }
}