using Microsoft.EntityFrameworkCore;
using Municipal_services_app.Models;
using MunicipalMvcApp.Models;

namespace MunicipalMvcApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Issue> Issues => Set<Issue>();
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Announcement> Announcements { get; set; } = null!;
        public DbSet<Recommendation> Recommendations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           //We are setting table names
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Event>().HasKey(e => e.EventID);
            modelBuilder.Entity<Announcement>().HasKey(a => a.AnnouncementID);
        }
    }
}
