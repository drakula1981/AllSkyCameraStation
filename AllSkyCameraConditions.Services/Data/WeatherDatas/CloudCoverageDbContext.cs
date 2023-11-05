using AllSkyCameraConditionService.Model.WeatherDatas;
using Microsoft.EntityFrameworkCore;

namespace AllSkyCameraConditions.Services.Data.WeatherDatas;
public class CloudCoverageDbContext : DbContext {
   public CloudCoverageDbContext(DbContextOptions<CloudCoverageDbContext> options) : base(options) { }

   protected CloudCoverageDbContext() { }

   public DbSet<CloudCoverture> CloudCovertures { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<CloudCoverture>().ToTable("CLOUD_COVERTURES").HasKey(c => c.Id);
}

