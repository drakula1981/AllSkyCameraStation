using AllSkyCameraConditionService.Model.WeatherDatas;
using Microsoft.EntityFrameworkCore;

namespace AllSkyCameraConditions.Services.Data;
public class SkyConditionsDbContext : DbContext {
   public SkyConditionsDbContext(DbContextOptions<SkyConditionsDbContext> options) : base(options) { }

   protected SkyConditionsDbContext() { }

   public DbSet<SkyConditions> SkyConditions { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<SkyConditions>().ToTable("SKY_CONDITIONS").HasKey(c => c.Id);
}

