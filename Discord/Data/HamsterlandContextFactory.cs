using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Hamsterland.MyAnimeList.Data
{
    public class HamsterlandContextFactory : IDesignTimeDbContextFactory<HamsterlandContext>
    {
        public HamsterlandContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<HamsterlandContext>()
                .Build();
            
            var optionsBuilder = new DbContextOptionsBuilder()
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseNpgsql(configuration["Postgres:Connection"]);
            
            return new HamsterlandContext(optionsBuilder.Options);
        }
    }
}