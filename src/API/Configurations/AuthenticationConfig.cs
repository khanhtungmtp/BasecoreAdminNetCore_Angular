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
        var validIssuer = configuration.GetValue<string>("JwtTokenSettings:ValidIssuer");
        var validAudience = configuration.GetValue<string>("JwtTokenSettings:ValidAudience");
        string? symmetricSecurityKey = configuration.GetValue<string>("JwtTokenSettings:SymmetricSecurityKey") ?? "";
        // add autentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                    .GetBytes(configuration.GetSection("Appsettings:Token")?.Value ?? "")),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        //2. Setup idetntity
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<DataContext>();

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
        services.AddAuthorization();
    }
}