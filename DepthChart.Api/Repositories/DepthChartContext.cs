using Microsoft.EntityFrameworkCore;
using DepthChart.Api.Models;


namespace DepthChart.Api.Repositories
{
    public class DepthChartDbContext : DbContext
    {
        public DbSet<Models.DepthChart> DepthCharts { get; set; }
        public DbSet<Models.ChartPositionDepth> ChartPositionDepths { get; set; }

        public DepthChartDbContext(DbContextOptions<DepthChartDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChartPositionDepth>()
                .HasKey(d => new { d.DepthChartId, d.PlayerId }); // Composite key
        }
    }
}
