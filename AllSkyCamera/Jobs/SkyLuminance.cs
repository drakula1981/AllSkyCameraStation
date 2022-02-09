using AllSkyCameraConditionService.GpioManaging;
using AllSkyCameraConditionService.Model;
using Quartz;
using Serilog;
using System.Text;

namespace AllSkyCameraConditionService.Jobs {
   internal class SkyLuminance : IJob {

      private static TSL2591? Sensor;

      private static bool IsInitialized => Sensor != null;
      private static async Task Initialize() {
         if (IsInitialized) return;
         Log.Logger.Debug("[SkyLuminance] Initializing luminance datas sensor");
         try {
            Sensor = new TSL2591();
            Log.Logger.Debug("[SkyLuminance] Sensor instance complete");
            await Sensor.SetGainAsync(TSL2591.GAIN_LOW, TSL2591.INT_TIME_100MS);
            Log.Logger.Debug("[SkyLuminance] Sensor gain/integration datas applied");
         } catch (Exception ex) {
            Log.Logger.Error(ex, "[SkyLuminance] Error while initializing luminance datas sensor");
         }
      }

      public async Task Execute(IJobExecutionContext context) => await StartRead();
      

      public static async Task StartRead() {
         try {
            await Initialize();
            if (null == Sensor) {
               Log.Logger.Error($"[SkyLuminance] Could not read luminance datas due to sensor non configuration");
               return;
            }
            float currentTemp = 0;
            if (null != DataHisto.Instance && null != DataHisto.Instance?.LastWeatherDatas) currentTemp = (float)DataHisto.Instance?.LastWeatherDatas?.Temperature;
            Log.Logger.Debug("[SkyLuminance] Starting read Sensor datas with {TSL2591.GAIN_LOW, TSL2591.INT_TIME_100MS}");
            var illuminanceDatas = await Sensor.GetLuxAsync(currentTemp);

            if (illuminanceDatas?.Mpsas == float.NaN || illuminanceDatas?.Mpsas == float.PositiveInfinity || illuminanceDatas?.Mpsas == float.NegativeInfinity) {
               Log.Logger.Debug("[SkyLuminance] Failed to correct Sensor datas, trying with {TSL2591.GAIN_MED, TSL2591.INT_TIME_100MS}");
               await Sensor.SetGainAsync(TSL2591.GAIN_MED, TSL2591.INT_TIME_100MS);
               illuminanceDatas = await Sensor.GetLuxAsync(currentTemp);
               if (illuminanceDatas?.Mpsas == float.NaN || illuminanceDatas?.Mpsas == float.PositiveInfinity || illuminanceDatas?.Mpsas == float.NegativeInfinity) {
                  Log.Logger.Debug("[SkyLuminance] Failed to correct Sensor datas, trying with {TSL2591.GAIN_HIGH, TSL2591.INT_TIME_100MS}");
                  await Sensor.SetGainAsync(TSL2591.GAIN_HIGH, TSL2591.INT_TIME_100MS);
                  illuminanceDatas = await Sensor.GetLuxAsync(currentTemp);
                  if (illuminanceDatas?.Mpsas == float.NaN || illuminanceDatas?.Mpsas == float.PositiveInfinity || illuminanceDatas?.Mpsas == float.NegativeInfinity) {
                     Log.Logger.Debug("[SkyLuminance] Failed to correct Sensor datas, trying with {TSL2591.GAIN_MAX, TSL2591.INT_TIME_100MS}");
                     await Sensor.SetGainAsync(TSL2591.GAIN_MAX, TSL2591.INT_TIME_100MS);
                     illuminanceDatas = await Sensor.GetLuxAsync(currentTemp);
                  }
               }
            }
            if (null != illuminanceDatas) DataHisto.Instance?.AddSkyQualityDatas(illuminanceDatas);
            if (AppParams.DebugMode) Log.Logger.Information($"[SkyLuminance] {illuminanceDatas}");
            await Sensor.SetGainAsync(TSL2591.GAIN_LOW, TSL2591.INT_TIME_100MS);
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
}
