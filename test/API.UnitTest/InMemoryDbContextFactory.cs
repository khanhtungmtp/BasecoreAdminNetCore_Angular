
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.UnitTest;
public class InMemoryDbContextFactory
{
    public DataContext GetDataContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
                   .UseInMemoryDatabase(databaseName: "InMemoryApplicationDatabase")
                   .Options;
        var dbContext = new DataContext(options);

        return dbContext;
    }
}
