using AllSkyCameraConditionService.Model.WeatherDatas;
using Microsoft.EntityFrameworkCore;

namespace AllSkyCameraConditions.Services.Data;
public class WeatherDbContext : DbContext {
   public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }

   protected WeatherDbContext() { }

   public DbSet<Weather> WeatherDatas { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<Weather>().ToTable("WEATHER_CONDITIONS").HasKey(c => c.Id);
}

