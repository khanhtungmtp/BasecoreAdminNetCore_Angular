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
        var logger = NLog.LogManager.GetLogger("applog");
        try
        {
            logger.Log(NLog.LogLevel.Info, "Seeding data...");
            var services = scope.ServiceProvider;
            var dbInitializer = services.GetRequiredService<DbInitializer>();
            dbInitializer.Seed().Wait();
            logger.Log(NLog.LogLevel.Info, "Seeding done!");
        }
        catch (Exception ex)
        {
            logger.Log(NLog.LogLevel.Error, $"An error occurred while seeding the database.: {ex}");
        }
    }
}
