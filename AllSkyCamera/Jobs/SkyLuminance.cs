using AllSkyCameraConditionService.GpioManaging;
using AllSkyCameraConditionService.Model;
using Quartz;
using Serilog;
using System.Text;

namespace AllSkyCameraConditionService.Jobs;
internal class SkyLuminance : IJob {

   private static TSL2591? Sensor;

   private static bool IsInitialized => Sensor != null;
   private static async Task Initialize() {
      if (IsInitialized) return;
      Log.Logger.Debug("[SkyLuminance] Initializing luminance datas sensor");
      try {
         Sensor = new TSL2591();
         Log.Logger.Debug("[SkyLuminance] Sensor instance complete");
         await Task.Delay(10);
      } catch (Exception ex) {
         Log.Logger.Error(ex, "[SkyLuminance] Error while initializing luminance datas sensor");
      }
   }

   public async Task Execute(IJobExecutionContext context) => await StartRead(DataHisto.Instance?.LastWeatherDatas?.Temperature);

   public static async Task StartRead(double? temperature) {
      try {
         await Initialize();
         if (null == Sensor) {
            Log.Logger.Error($"[SkyLuminance] Could not read luminance datas due to sensor non configuration");
            return;
         }
         float currentTemp = (float)(temperature ?? 0);
         var times = new Dictionary<int, byte>() {
               {0, TSL2591.INT_TIME_100MS },
               {1, TSL2591.INT_TIME_200MS },
               {2, TSL2591.INT_TIME_300MS },
               {3, TSL2591.INT_TIME_400MS },
               {4, TSL2591.INT_TIME_500MS },
               {5, TSL2591.INT_TIME_600MS }
            };
         SkyConditions? illuminanceDatas = null;
         bool endReading = false;
         for (var t = 0; t < times.Count; t++) {
            if (endReading) continue;
            if (AppParams.DebugMode) Log.Logger.Information($"[SkyLuminance] Trying to read Sensor datas with [g:{TSL2591.GAIN_MAX}/9876, ti:{(t + 1) * 100},temp:{currentTemp}]");
            await Sensor.SetGainAsync(TSL2591.GAIN_MAX, times[t]);
            illuminanceDatas = await Sensor.GetLuxAsync(currentTemp);
            endReading = illuminanceDatas != null && !(double.IsNaN(illuminanceDatas.Mpsas) || double.IsInfinity(illuminanceDatas.Mpsas) || illuminanceDatas.VisibleLight < 0);
         }

         if (illuminanceDatas != null) {
            DataHisto.Instance?.AddSkyQualityDatas(illuminanceDatas);
            if (AppParams.DebugMode) Log.Logger.Information($"[SkyLuminance] {illuminanceDatas}");
         } else {
            if (AppParams.DebugMode) Log.Logger.Information($"[SkyLuminance] illuminanceDatas empty");
         }
      } catch (Exception ex) {
         Log.Logger.Error(ex, "[SkyLuminance] Error while reading luminance datas sensor");
      }
   }
   public static async Task ToCsv(FileInfo destFile) {
      StringBuilder sb = new(destFile.Exists ? $"date;fullspectrum;infrared;visiblelight;integrated;gain;integration_time;mpsas;dmpsas{Environment.NewLine}" : "");
      DataHisto.Instance.SkyDatasHisto.ForEach(w => sb.AppendLine(w.ToCsv()));
      byte[] buffer = Encoding.Unicode.GetBytes(sb.ToString());
      using FileStream fs = destFile.Open(FileMode.OpenOrCreate);
      await fs.WriteAsync(buffer);
      fs.Flush();
      fs.Close();
   }
}