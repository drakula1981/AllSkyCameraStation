using AllSkyCameraConditionService.Model;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using System.Text;

namespace AllSkyCameraConditionService.Jobs {
   internal class HistoryLogger : IJob {

      public async Task Execute(IJobExecutionContext context) {
         JobDataMap dataMap = context.JobDetail.JobDataMap;
         if (null == dataMap) throw new ArgumentException("dataMap is empty");
         var SunRiseSetDatas = dataMap.GetString("SunRiseSetDatas") ?? String.Empty;
         await SaveJsonLog(AppParams.DatasHistoryFilePath);
         await SaveWeatherFile(AppParams.WeatherDatasFilePath);
         await SaveCloudWatchFile(AppParams.CloudWatcherDatasFilePath);
         await SaveSkyQualityDatasFile(AppParams.SkyQualityDatasFilePath);
         await SaveAllSkyDatasFile(AppParams.AllSkyDatasFilePath, AppParams.AllSkyWebUIDatasFilePath, JsonConvert.DeserializeObject<Root>(SunRiseSetDatas));
      }

      private static async Task SaveJsonLog(FileInfo destFile) {
         if (destFile.Exists && destFile.LastWriteTime.Date < DateTime.Now.Date) {
            destFile.MoveTo($"{destFile.DirectoryName}/{destFile.LastWriteTime:yyyyMMdd}_{destFile.Name}");
         } else destFile.Delete();
         try {
            var weatherGroupedDatas = DataHisto.Instance.WeatherDatasHisto.GroupBy(w => new { w.MeasureDate.Date, w.MeasureDate.Hour });
            var cloudGroupedDatas = DataHisto.Instance.CloudWatchHisto.GroupBy(w => new { w.MeasureDate.Date, w.MeasureDate.Hour });
            var skyGroupedDatas = DataHisto.Instance.SkyDatasHisto.GroupBy(w => new { w.MeasureDate.Date, w.MeasureDate.Hour });
            var histo = new {
               weatherAveregedDatas = weatherGroupedDatas.Select(g => new WeatherDatas(new DateTime(g.Key.Date.Year, g.Key.Date.Month, g.Key.Date.Day, g.Key.Hour, 0, 0), g.Average(t => t.Temperature), g.Average(h => h.Humidity), g.Average(p => p.Pressure))).OrderBy(w => w.MeasureDate).ToList(),
               cloudAveregedDatas = cloudGroupedDatas.Select(g => new CloudTemperatureDatas(new DateTime(g.Key.Date.Year, g.Key.Date.Month, g.Key.Date.Day, g.Key.Hour, 0, 0), g.Average(t => t.AmbientTemperature), g.Average(h => h.MlxAmbientTemperature), g.Average(p => p.SkyTemperature))).OrderBy(w => w.MeasureDate).ToList(),
               skyAveregedDatas = skyGroupedDatas.Select(g => new SkyConditions(new DateTime(g.Key.Date.Year, g.Key.Date.Month, g.Key.Date.Day, g.Key.Hour, 0, 0), g.Average(t => t.VisibleLight), g.Average(h => h.Infrared), g.Average(p => p.FullSpectrum), g.Average(p => p.Integrated), (int)g.Average(p => p.Gain), (int)g.Average(p => p.IntegrationTime))).OrderBy(w => w.MeasureDate).ToList()
            };

            StringBuilder sb = new(JsonConvert.SerializeObject(histo));
            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            using FileStream fs = destFile.Open(FileMode.OpenOrCreate);
            await fs.WriteAsync(buffer);
            fs.Flush();
            fs.Close();
            Log.Logger.Information($"[HistoryLogger] destFile[{destFile}] delivered");
            sb = sb.Clear().Append($"{{\"LastCloudWatch\":{DataHisto.Instance.LastCloudWatch},\"LastWeatherDatas\":{DataHisto.Instance.LastWeatherDatas}, \"LastSkyQualityDatas\":{DataHisto.Instance.LastSkyDatas},\"MoonDatas\":{Moon.Now()}}}");
            buffer = Encoding.UTF8.GetBytes(sb.ToString());
            var currentInfosJson = new FileInfo($"{destFile.DirectoryName}/current.json");
            if (currentInfosJson.Exists) currentInfosJson.Delete();
            using FileStream fs2 = currentInfosJson.Open(FileMode.OpenOrCreate);
            await fs2.WriteAsync(buffer);
            fs2.Flush();
            fs2.Close();
         } catch (InvalidOperationException) {
            Log.Logger.Error($"[HistoryLogger] Unable to write all the datas in the json file due mainly by adding some datas in the logger.");
         }
      }

      private static async Task SaveAllSkyDatasFile(FileInfo destFile, FileInfo webUIDestFile, Root? sunDatas) {
         var lastWeather = DataHisto.Instance.LastWeatherDatas;
         var skyDatas = DataHisto.Instance.LastSkyDatas;
         var cloudWatch = DataHisto.Instance.LastCloudWatch;

         int dawn = int.TryParse(sunDatas.Rising.Hour[..2], out int h) ? h : 12;
         int dusk = int.TryParse(sunDatas.Setting.Hour[..2], out h) ? h : 12;
         StringBuilder sb = new();

         sb.AppendLine($"Temperature: {lastWeather.Temperature} C")
           .AppendLine($"Pressure: {lastWeather.Pressure} hPa")
           .AppendLine($"Humidity: {lastWeather.Humidity} %");
         if (!DateTime.Now.Hour.IsWithin(dawn, dusk)) sb.AppendLine($"SQM: {skyDatas.Mpsas} Mpsas")
                                                        .AppendLine($"AstroDawn: {sunDatas.DawnAstronomical.Hour}")
                                                        .AppendLine($"Moon age: {Moon.Now().MoonAge}");
         else sb.AppendLine($"AstroDusk: {sunDatas.DuskAstronomical.Hour}");
         byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
         using FileStream fs = destFile.Open(destFile.Exists ? FileMode.Truncate : FileMode.OpenOrCreate);
         await fs.WriteAsync(buffer);
         fs.Flush();
         fs.Close();
         sb = sb.Clear();
         sb.AppendLine($"data\t{AppParams.MeasureInterval * 60}\tTemperature\t{lastWeather.Temperature}°C");
         sb.AppendLine($"data\t{AppParams.MeasureInterval * 60}\tPressure\t{lastWeather.Pressure}hPa");
         sb.AppendLine($"progress\t{AppParams.MeasureInterval * 60}\tDew Point\t{lastWeather.DewPoint}°C\t{DataHisto.Instance.WeatherDatasHisto.Where(h => h.MeasureDate.ToShortDateString().Equals(DateTime.Now.ToShortDateString())).Min(w => w.DewPoint)}\t{lastWeather.DewPoint}\t{DataHisto.Instance.WeatherDatasHisto.Where(h => h.MeasureDate.ToShortDateString().Equals(DateTime.Now.ToShortDateString())).Max(w => w.DewPoint)}\t10\t15");
         sb.AppendLine($"progress\t{AppParams.MeasureInterval * 60}\tHumidity\t{lastWeather.Humidity}%\t0\t{lastWeather.Humidity}\t100\t85\t75");
         if (!DateTime.Now.Hour.IsWithin(dawn, dusk)) sb.AppendLine($"progress\t{AppParams.MeasureInterval * 60}\tCloud Percent\t{cloudWatch.CloudCoveragePercent}%\t0\t{cloudWatch.CloudCoveragePercent}\t100\t30\t20");
         buffer = Encoding.UTF8.GetBytes(sb.ToString());
         using FileStream fs2 = webUIDestFile.Open(webUIDestFile.Exists ? FileMode.Truncate : FileMode.OpenOrCreate);
         await fs2.WriteAsync(buffer);
         fs2.Flush();
         fs2.Close();
      }
      private static async Task SaveWeatherFile(FileInfo destFile) => await Conditions.ToCsv(destFile);
      private static async Task SaveCloudWatchFile(FileInfo destFile) => await CloudSensor.ToCsv(destFile);
      private static async Task SaveSkyQualityDatasFile(FileInfo destFile) => await SkyLuminance.ToCsv(destFile);

   }
}
