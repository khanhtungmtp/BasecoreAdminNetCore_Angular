using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace API.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environmentName}.json")
            .Build();
        var area = configuration.GetSection("AppSettings:Area").Value;
        var builder = new DbContextOptionsBuilder<DataContext>();
        var connectionString = configuration.GetConnectionString($"DefaultConnection_{area}");
        builder.UseSqlServer(connectionString);
        return new DataContext(builder.Options);
    }
}