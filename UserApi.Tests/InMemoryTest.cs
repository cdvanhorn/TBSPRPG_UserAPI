using Microsoft.EntityFrameworkCore;
using UserApi.Repositories;

namespace UserApi.Tests
{
    public class InMemoryTest
    {
        protected readonly DbContextOptions<UserContext> _dbContextOptions;

        protected InMemoryTest(string dbName)
        {
            _dbContextOptions = new DbContextOptionsBuilder<UserContext>()
                    .UseInMemoryDatabase(dbName)
                    .Options;
        }
    }
}