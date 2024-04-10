using API.Data;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using API.Controllers.UserManager;
using ViewModels.UserManager;
using Microsoft.AspNetCore.Mvc;
using API.Helpers.Utilities;
using MockQueryable.Moq;
using API._Services.Interfaces.UserManager;

namespace API.UnitTest;

public class UsersControllerTest
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<I_User> _user;
    private DataContext _context;

    private List<User> _userSources = new List<User>(){
                         new("1","test1","Test 1","test1@gmail.com","001111",DateTime.Now),
                         new("2","test2","Test 2","test2@gmail.com","001111",DateTime.Now),
                         new("3","test3","Test 3","test3@gmail.com","001111",DateTime.Now),
                         new("4","test4","Test 4","test4@gmail.com","001111",DateTime.Now),
                    };

    public UsersControllerTest()
    {
        var userStore = new Mock<IUserStore<User>>();
        var optionsAccessor = new Mock<IOptions<IdentityOptions>>();
        var passwordHasher = new Mock<IPasswordHasher<User>>();
        var userValidators = new List<IUserValidator<User>>();
        var passwordValidators = new List<IPasswordValidator<User>>();
        var keyNormalizer = new Mock<ILookupNormalizer>();
        var errors = new Mock<IdentityErrorDescriber>();
        var services = new Mock<IServiceProvider>();
        var logger = new Mock<ILogger<UserManager<User>>>();
        _user = new Mock<I_User>();
        _mockUserManager = new Mock<UserManager<User>>(
            userStore.Object,
            optionsAccessor.Object,
            passwordHasher.Object,
            userValidators.ToArray(),
            passwordValidators.ToArray(),
            keyNormalizer.Object,
            errors.Object,
            services.Object,
            logger.Object);
        _context = new InMemoryDbContextFactory().GetDataContext();
    }

    // check exits controller
    [Fact]
    public void CreateInstance_NotNull()
    {
        var usersController = new UsersController(_mockUserManager.Object, _user.Object);
        Assert.NotNull(usersController);
    }

    // create user success
    [Fact]
    public async Task CreateUser_ValidInput_Success()
    {
        // gia lap create thanh cong
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        // setup return user
        _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new User() { FullName = "test1" });
        //Act
        var usersController = new UsersController(_mockUserManager.Object, _user.Object);
        // start create user
        var result = await usersController.CreateUser(new UserCreateRequest() { FullName = "test" });
        //  Đảm bảo rằng kết quả trả về từ phương thức CreateUser không phải là null
        Assert.NotNull(result);
        // Kiểm tra để xác nhận rằng kết quả trả về là một CreatedAtActionResult
        Assert.IsType<CreatedAtActionResult>(result);
    }

    // create user failed
    [Fact]
    public async Task CreateUser_ValidInput_Failed()
    {
        // gia lap create that bai
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError[] { }));
        // setup return user
        _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new User() { FullName = "test1" });
        //Act
        var usersController = new UsersController(_mockUserManager.Object, _user.Object);
        // start create user
        var result = await usersController.CreateUser(new UserCreateRequest() { FullName = "test" });
        //  Đảm bảo rằng kết quả trả về từ phương thức CreateUser không phải là null
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    // getallpaging success
    [Fact]
    public async Task GetAllPaging_ReturnsOkObject_WithPagedResults()
    {
        var users = new List<User>
        {
            new() { FullName = "Test User 1", Email = "test1@example.com", DateOfBirth = DateTime.Parse("2000-01-01") },
            new() { FullName = "Test User 2", Email = "test2@example.com", DateOfBirth = DateTime.Parse("1990-01-01") },
            // Add more users for testing
        }.AsQueryable();
        //Sử dụng dữ liệu giả lập cho các thực thể người dùng (User entities).
        _mockUserManager.Setup(m => m.Users).Returns(_userSources.AsQueryable().BuildMock());

        var paginationParam = new PaginationParam { PageNumber = 1, PageSize = 10 };
        var userVM = new UserVM();
        var usersController = new UsersController(_mockUserManager.Object, _user.Object);

        // Act
        /*
        Gọi phương thức GetAllPaging và kiểm tra xem nó có trả về OkObjectResult 
        với kết quả đã phân trang (PagingResult<UserVM>) chứa số lượng phần tử thích hợp với PageSize được đưa ra hay không.
        */
        var result = await usersController.GetPaging("test1", paginationParam, userVM);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<PagingResult<UserVM>>(actionResult.Value);
        Assert.True(returnValue.Result.Count <= paginationParam.PageSize);
    }

    // getallpaging failed
    [Fact]
    public async Task GetAllPaging_ThrowException_Failed()
    {
        // gia lap get loi
        _mockUserManager.Setup(x => x.Users).Throws<Exception>();
        var paginationParam = new PaginationParam { PageNumber = 1, PageSize = 10 };
        var userVM = new UserVM();
        var usersController = new UsersController(_mockUserManager.Object, _user.Object);
        await Assert.ThrowsAnyAsync<Exception>(async () => await usersController.GetPaging("test1", paginationParam, userVM));
    }

    // get by id user
    [Fact]
    public async Task GetById_ReturnsOkObjectResult_WithUser()
    {
        // setup return user
        _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User() { FullName = "test1" });
        var usersController = new UsersController(_mockUserManager.Object, _user.Object);
        var result = await usersController.GetById("1");
        var okResult = result as OkObjectResult;
        var userVM = okResult?.Value as UserVM;
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal("test1", userVM?.FullName);
    }

    // get by id user failed
    [Fact]
    public async Task GetById_ThrowException_Failed()
    {
        // gia lap get loi
        _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).Throws<Exception>();
        var usersController = new UsersController(_mockUserManager.Object, _user.Object);
        await Assert.ThrowsAnyAsync<Exception>(async () => await usersController.GetById("1"));
    }
}