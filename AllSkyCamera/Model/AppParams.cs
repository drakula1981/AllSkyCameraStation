using System.Configuration;

namespace AllSkyCameraConditionService.Model {
   public static class AppParams {
      public static int WeatherGpioID => int.Parse(ConfigurationManager.AppSettings["SensorGpioID"] ?? "0");
      public static int HeaterGpioID => int.Parse(ConfigurationManager.AppSettings["HeaterGpioID"] ?? "0");
      public static int FanGpioID => int.Parse(ConfigurationManager.AppSettings["FanGpioID"] ?? "0");
      public static int MeasureInterval => int.Parse(ConfigurationManager.AppSettings["MeasureInterval"] ?? "10");
      public static int MaxHeatingTemp => int.Parse(ConfigurationManager.AppSettings["MaxHeatingTemp"] ?? "25");
      public static int HeatingTime => int.Parse(ConfigurationManager.AppSettings["HeatingTime"] ?? "5");
      public static bool DebugMode => bool.Parse(ConfigurationManager.AppSettings["Debug"] ?? "true");
      public static FileInfo WeatherDatasFilePath => new(ConfigurationManager.AppSettings["WeatherDatasPath"] ?? "/datas/wdp.csv");
      public static FileInfo CloudWatcherDatasFilePath => new(ConfigurationManager.AppSettings["CloudWatcherDatasFilePath"] ?? "/datas/cwdfp.csv");
      public static FileInfo DatasHistoryFilePath => new(ConfigurationManager.AppSettings["DatasHistoryFilePath"] ?? "/datas/dh.json");
      public static FileInfo SkyQualityDatasFilePath => new(ConfigurationManager.AppSettings["SkyQualityDatasFilePath"] ?? "/datas/sqd.json");
      public static string DbConnectionString => ConfigurationManager.AppSettings["DbConnectionString"] ?? "/datas/service.db";
      public static string ApiAccessUrl => ConfigurationManager.AppSettings["ApiAccessUrl"] ?? "http://localhost:4242";

   }
}
