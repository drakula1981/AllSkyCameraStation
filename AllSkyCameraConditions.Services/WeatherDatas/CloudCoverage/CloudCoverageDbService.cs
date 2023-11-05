using AllSkyCameraConditions.Interfaces.WeatherDatas.CloudCoverage;
using AllSkyCameraConditions.Services.Data.WeatherDatas;
using AllSkyCameraConditionService.Model.WeatherDatas;

namespace AllSkyCameraConditions.Services.WeatherDatas.CloudCoverage;
internal class CloudCoverageDbService(CloudCoverageDbContext dbContext) : ICloudCoverageDbService {
   private readonly CloudCoverageDbContext _dbContext = dbContext;

   public void AddMeasure(CloudCoverture? ent) {
      if (null != ent) {
         _dbContext.CloudCovertures.Add(ent);
         _dbContext.SaveChanges();
      }
   }

   public List<CloudCoverture> GetAll() => _dbContext.CloudCovertures.ToList();

   public CloudCoverture? GetById(Guid id) => _dbContext.CloudCovertures.Find(id);

   public List<CloudCoverture> GetByDate(DateTime date) => _dbContext.CloudCovertures.Where(c => c.MeasureDate.Equals(date)).ToList();

   public List<CloudCoverture> GetByDates(DateTime dateStart, DateTime dateEnd) => _dbContext.CloudCovertures.Where(c => c.MeasureDate >= dateStart && c.MeasureDate <= dateEnd).ToList();
}
