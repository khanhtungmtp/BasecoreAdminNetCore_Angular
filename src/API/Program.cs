using System.Net;
using API.Configurations;
using API.Data;
using API.Helpers.Base;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NLog.Web;
using ViewModels.UserManager.Validator;
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseNLog();

    // Add services to the container.
    builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
     options.InvalidModelStateResponseFactory = actionContext =>
     {
         var context = actionContext.HttpContext;
         string? trackId = Guid.NewGuid().ToString();
         context.Response.Headers.Append("TrackId", trackId); // add TrackId to header response

         var modelState = actionContext.ModelState.Values;
         var errors = modelState.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);

         return new BadRequestObjectResult(new ErrorGlobalResponse
         {
             TrackId = trackId,
             Title = ReasonPhrases.GetReasonPhrase((int)HttpStatusCode.BadRequest),
             Status = (int)(HttpStatusCode)context.Response.StatusCode,
             Errors = errors,
             Instance = $"{context.Request.Method} {context.Request.Path}",
         });
     }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });
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
    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddSwaggerGen();
    }
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
    // seeding inittial Data
    // DataSeeder.SeedDatabase(app);
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
