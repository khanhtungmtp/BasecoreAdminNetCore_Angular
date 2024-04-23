using Serilog;

namespace API.Data;

public static class DataSeeder
{
    public static void SeedDatabase(IHost app)
    {
        using var scope = app.Services.CreateScope();
        var logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        if (!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
        }
        try
        {
            var services = scope.ServiceProvider;
            var dbInitializer = services.GetRequiredService<DbInitializer>();
            dbInitializer.Seed().Wait();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DataSeeder error: {Message}", ex.Message);
        }
    }
}
