using LiteDB;
using Newtonsoft.Json;

namespace AllSkyCameraConditionService.Model {
   [JsonObject("DataHisto")]
   internal class DataHisto: IDisposable {
      [JsonIgnore] private readonly LiteDatabase db = new(AppParams.DbConnectionString);
      [JsonIgnore] private ILiteCollection<WeatherDatas> WeatherDatasDb => db.GetCollection<WeatherDatas>("weatherDatas");
      [JsonIgnore] private ILiteCollection<CloudTemperatureDatas> CloudWatchDb => db.GetCollection<CloudTemperatureDatas>("cloudWatchingDatas");
      [JsonIgnore] private ILiteCollection<CpuTemperatureDatas> CpuTempDb => db.GetCollection<CpuTemperatureDatas>("cpuTemperatureDatas");

      [JsonProperty("lastWeatherDatas")] public WeatherDatas? LastWeatherDatas => WeatherDatasDb.Query().OrderByDescending(w => w.MeasureDate).FirstOrDefault();
      [JsonProperty("weatherDatasHisto")] public List<WeatherDatas> WeatherDatasHisto => WeatherDatasDb.Query().OrderByDescending(w => w.MeasureDate).ToList();
      [JsonProperty("lastCloudWatch")] public CloudTemperatureDatas? LastCloudWatch => CloudWatchDb.Query().OrderByDescending(w => w.MeasureDate).FirstOrDefault();
      [JsonProperty("cloudWatchHisto")] public List<CloudTemperatureDatas> CloudWatchHisto => CloudWatchDb.Query().OrderByDescending(w => w.MeasureDate).ToList();
      [JsonProperty("lastCpuTempDatas")] public CpuTemperatureDatas? LastCpuTempDatas => CpuTempDb.Query().OrderByDescending(w => w.MeasureDate).FirstOrDefault();
      [JsonProperty("cpuTempHisto")] public List<CpuTemperatureDatas> CpuTempHisto => CpuTempDb.Query().OrderByDescending(w => w.MeasureDate).ToList();

      public static DataHisto Instance = new();
      private DataHisto() { }

      public void AddWeatherDatas(WeatherDatas datas) => WeatherDatasDb.Insert(datas);
      public void AddCloudWatcherDatas(CloudTemperatureDatas datas) => CloudWatchDb.Insert(datas);
      public void AddCpuTempDatas(CpuTemperatureDatas datas) => CpuTempDb.Insert(datas);
      public CurrentConditions GetCurrentConditions() => new() { Temperature = LastWeatherDatas?.Temperature,
                                                                 Humidity = LastWeatherDatas?.Humidity,
                                                                 Pressure = LastWeatherDatas?.Pressure,
                                                                 DewPoint = LastWeatherDatas?.DewPoint,
                                                                 MeasureDate = LastWeatherDatas?.MeasureDate,
                                                                 SkyTemperature = LastCloudWatch?.SkyTemperature,
                                                                 CloudCoveragePercent = LastCloudWatch?.CloudCoveragePercent,
                                                                 IsSafe = LastCloudWatch?.IsSafe};

      public override string ToString() => JsonConvert.SerializeObject(this);

      public void Dispose() => Dispose(true);

      public void Dispose(bool disposing) {
         if (!disposing) return;
         db.Rebuild();
         db.Dispose();
      }
   }
}
