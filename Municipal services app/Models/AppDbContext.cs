using Microsoft.EntityFrameworkCore;
using MunicipalMvcApp.Models;

namespace MunicipalMvcApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Issue> Issues => Set<Issue>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configs supplémentaires si besoin
        }
    }
}
