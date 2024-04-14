using System.Text;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
namespace API.Configurations;
public static class AuthenticationConfig
{
    public static void AddAuthenticationConfigufation(this IServiceCollection services, IConfiguration configuration)
    {
        // These will eventually be moved to a secrets file, but for alpha development appsettings is fine
        string? secretKey = configuration["AppSettings:SecretKey"];
        ArgumentNullException.ThrowIfNull(secretKey);
        var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
        // add autentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                //tự cấp token
                ValidateIssuer = false,
                ValidateAudience = false,

                //ký vào token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                ClockSkew = TimeSpan.Zero
            };
        });

        //2. Setup idetntity
        services.AddIdentityCore<User>()
           .AddRoles<IdentityRole>() // Nếu bạn muốn sử dụng Roles
           .AddEntityFrameworkStores<DataContext>() // Set up EF stores
           .AddSignInManager<SignInManager<User>>() // Thêm SignInManager nếu bạn cần nó
           .AddDefaultTokenProviders(); // Thêm token providers nếu bạn muốn sử dụng function như là đặt lại mật khẩu

        services.Configure<IdentityOptions>(options =>
        {
            // Default Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.User.RequireUniqueEmail = true;
        });
    }
}