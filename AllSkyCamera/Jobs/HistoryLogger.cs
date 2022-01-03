using AllSkyCameraConditionService.Model;
using Quartz;
using Serilog;
using System.Text;

namespace AllSkyCameraConditionService.Jobs {
   internal class HistoryLogger : IJob {
      public async Task Execute(IJobExecutionContext context) {
         await SaveJsonLog(AppParams.DatasHistoryFilePath);
         await SaveWeatherFile(AppParams.WeatherDatasFilePath);
         await SaveCloudWatchFile(AppParams.CloudWatcherDatasFilePath);
      }

      private static async Task SaveJsonLog(FileInfo destFile) {
         if (destFile.Exists && destFile.LastWriteTime.Date < DateTime.Now.Date) {
            destFile.MoveTo($"{destFile.DirectoryName}/{destFile.LastWriteTime:yyyyMMdd}_{destFile.Name}");
         } else destFile.Delete();
         try {
            StringBuilder sb = new($"{DataHisto.Instance}");
            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            using FileStream fs = destFile.Open(FileMode.OpenOrCreate);
            await fs.WriteAsync(buffer);
            fs.Flush();
            fs.Close();
            sb = sb.Clear().Append($"{{\"LastCloudWatch\":{DataHisto.Instance.LastCloudWatch},\"LastWeatherDatas\":{DataHisto.Instance.LastWeatherDatas}}}");
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

   }
}
