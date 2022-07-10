using AllSkyCameraConditionService.Model;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using System.Text;

namespace AllSkyCameraConditionService.Jobs {
   internal class HistoryLogger : IJob {
      public async Task Execute(IJobExecutionContext context) {
         await SaveJsonLog(AppParams.DatasHistoryFilePath);
         await SaveWeatherFile(AppParams.WeatherDatasFilePath);
         await SaveCloudWatchFile(AppParams.CloudWatcherDatasFilePath);
         await SaveSkyQualityDatasFile(AppParams.SkyQualityDatasFilePath);
      }

      private static async Task SaveJsonLog(FileInfo destFile) {
         if (destFile.Exists && destFile.LastWriteTime.Date < DateTime.Now.Date) {
            destFile.MoveTo($"{destFile.DirectoryName}/{destFile.LastWriteTime:yyyyMMdd}_{destFile.Name}");
         } else destFile.Delete();
         try {

            var weatherGroupedDatas = DataHisto.Instance.WeatherDatasHisto.GroupBy(w => new { w.MeasureDate.Date, w.MeasureDate.Hour });
            var cloudGroupedDatas = DataHisto.Instance.CloudWatchHisto.GroupBy(w => new { w.MeasureDate.Date, w.MeasureDate.Hour });
            var skyGroupedDatas = DataHisto.Instance.SkyDatasHisto.GroupBy(w => new { w.MeasureDate.Date, w.MeasureDate.Hour });

            /*Log.Logger.Information($"[HistoryLogger] weatherGroupedDatas preparation = {JsonConvert.SerializeObject(weatherGroupedDatas)}");
            Log.Logger.Information($"[HistoryLogger] cloudGroupedDatas preparation   = {JsonConvert.SerializeObject(cloudGroupedDatas)}");
            Log.Logger.Information($"[HistoryLogger] skyGroupedDatas preparation     = {JsonConvert.SerializeObject(skyGroupedDatas)}");*/

            var histo = new {
               weatherAveregedDatas = weatherGroupedDatas.Select(g => new WeatherDatas(new DateTime(g.Key.Date.Year, g.Key.Date.Month, g.Key.Date.Day, g.Key.Hour, 0, 0), g.Average(t => t.Temperature), g.Average(h => h.Humidity), g.Average(p => p.Pressure))).OrderBy(w => w.MeasureDate).ToList(),
               cloudAveregedDatas = cloudGroupedDatas.Select(g => new CloudTemperatureDatas(new DateTime(g.Key.Date.Year, g.Key.Date.Month, g.Key.Date.Day, g.Key.Hour, 0, 0), g.Average(t => t.AmbientTemperature), g.Average(h => h.MlxAmbientTemperature), g.Average(p => p.SkyTemperature))).OrderBy(w => w.MeasureDate).ToList(),
               skyAveregedDatas = skyGroupedDatas.Select(g => new SkyConditions(new DateTime(g.Key.Date.Year, g.Key.Date.Month, g.Key.Date.Day, g.Key.Hour, 0, 0), g.Average(t => t.VisibleLight), g.Average(h => h.Infrared), g.Average(p => p.FullSpectrum), g.Average(p => p.Integrated), (int)g.Average(p => p.Gain), (int)g.Average(p => p.IntegrationTime))).OrderBy(w => w.MeasureDate).ToList()
            };

            //StringBuilder sb = new($"{DataHisto.Instance}");
            StringBuilder sb = new(JsonConvert.SerializeObject(histo));
            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            using FileStream fs = destFile.Open(FileMode.OpenOrCreate);
            await fs.WriteAsync(buffer);
            fs.Flush();
            fs.Close();
            Log.Logger.Information($"[HistoryLogger] destFile[{destFile}] delivered");
            sb = sb.Clear().Append($"{{\"LastCloudWatch\":{DataHisto.Instance.LastCloudWatch},\"LastWeatherDatas\":{DataHisto.Instance.LastWeatherDatas}, \"LastSkyQualityDatas\":{DataHisto.Instance.LastSkyDatas}}}");
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

      private static async Task SaveWeatherFile(FileInfo destFile) => await Conditions.ToCsv(destFile);
      private static async Task SaveCloudWatchFile(FileInfo destFile) => await CloudSensor.ToCsv(destFile);
      private static async Task SaveSkyQualityDatasFile(FileInfo destFile) => await SkyLuminance.ToCsv(destFile);

   }
}
