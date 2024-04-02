using API.Configurations;
using API.Data;
using API.Helpers.Base;
using FluentValidation;
using FluentValidation.AspNetCore;
using NLog.Web;
using ViewModels.UserManager.Validator;
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseNLog();

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssemblyContaining<RoleVmValidator>();
    // Setting DBContexts
    builder.Services.AddDatabaseConfiguration(builder.Configuration);

    // Add Authentication
    builder.Services.AddAuthenticationConfigufation(builder.Configuration);

    // RepositoryAccessor and Service
    builder.Services.AddDependencyInjectionConfiguration(typeof(Program));
    builder.Services.AddTransient<DbInitializer>();
    // Swagger Config
    builder.Services.AddSwaggerGenConfiguration();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseStaticFiles();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.UseExceptionHandler();
    // Tùy chỉnh seeding ở đây
    using (var scope = app.Services.CreateScope())
    {
        var logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        if (!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
        }
        var logger = NLog.LogManager.GetLogger("applog");
        try
        {
            logger.Log(NLog.LogLevel.Info, "Seeding data...");
            var services = scope.ServiceProvider;
            var dbInitializer = services.GetService<DbInitializer>();
            dbInitializer?.Seed().Wait();
            logger.Log(NLog.LogLevel.Info, "Seeding done!");
        }
        catch (Exception ex)
        {
            logger.Log(NLog.LogLevel.Error, $"An error occurred while seeding the database.: {ex}");
        }
    }
    app.Run();
}
catch
{
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
