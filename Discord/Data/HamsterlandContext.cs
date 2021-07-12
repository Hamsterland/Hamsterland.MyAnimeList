using Hamsterland.MyAnimeList.Models;
using Microsoft.EntityFrameworkCore;

namespace Hamsterland.MyAnimeList.Data
{
    public class HamsterlandContext : DbContext
    {
        public DbSet<MalAccount> MalAccounts { get; set; }
        
        public HamsterlandContext(DbContextOptions options) : base(options)
        { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);
        }
    }
}