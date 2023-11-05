using AllSkyCameraConditionService.Model.WeatherDatas;
using Microsoft.EntityFrameworkCore;

namespace AllSkyCameraConditions.Services.Data;
public class CurrentConditionsDbContext : DbContext {
   public CurrentConditionsDbContext(DbContextOptions<CurrentConditionsDbContext> options) : base(options) { }

   protected CurrentConditionsDbContext() { }

   public DbSet<CurrentConditions> CurrentConditions { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<CurrentConditions>().ToTable("INSTANT_MEASURES").HasKey(c => c.Id);
}

