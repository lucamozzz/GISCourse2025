using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace GISCourse2025.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=gis-project;Username=postgres;Password=postgres",
                o => o.UseNetTopologySuite());
        }
        public DbSet<GISCourse2025.Models.hikingTrails> hikingTrails { get; set; }
    }
}