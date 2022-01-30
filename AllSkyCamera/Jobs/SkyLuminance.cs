using AllSkyCameraConditionService.GpioManaging;
using AllSkyCameraConditionService.Model;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllSkyCameraConditionService.Jobs {
   internal class SkyLuminance : IJob {

      private static TSL2591? Sensor;
      private static bool IsInitialized => Sensor != null;
      private static async Task Initialize() {
         if (IsInitialized) return;
         try {
            Sensor = new TSL2591();
            await Sensor.SetGainAsync(TSL2591.GAIN_MAX, TSL2591.INT_TIME_600MS);
         } catch (Exception ex) {
            Log.Logger.Error(ex, "[SkyLuminance] Error while intializing luminance datas sensor");
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
            var illuminanceDatas = await Sensor.GetLuxAsync();

         } catch (Exception) {

            throw;
         }
      }
      public static async Task ToCsv(FileInfo destFile) {
         StringBuilder sb = new(destFile.Exists ? $"date;fullspectrum;infrared;visiblelight;integrated;mpsas;dmpsas{Environment.NewLine}" : "");
         DataHisto.Instance.SkyDatasHisto.ForEach(w => sb.AppendLine(w.ToCsv()));
         byte[] buffer = Encoding.Unicode.GetBytes(sb.ToString());
         using FileStream fs = destFile.Open(FileMode.OpenOrCreate);
         await fs.WriteAsync(buffer);
         fs.Flush();
         fs.Close();
      }

   }
}
