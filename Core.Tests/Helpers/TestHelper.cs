using System.IO;
using Core.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core.Tests.Helpers
{
    public static class TestHelper
    {
        public static IConfiguration InitializeConfiguration()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json")

                .Build();
            return config;
        }

        public static CoreContext GetDatabase()
        {
            var config = InitializeConfiguration();
            var options = config["Initialization:CoreConnectionString"];
            var dbContextOptions = new DbContextOptionsBuilder<CoreContext>().UseSqlServer(options);
            return new CoreContext(dbContextOptions.Options);

        }
    }
}
