using AllSkyCameraConditionService.GpioManaging;
using AllSkyCameraConditionService.Model;
using Iot.Device.CpuTemperature;
using Quartz;
using Serilog;

namespace AllSkyCameraConditionService.Jobs {
   internal class CpuTempMonitor : IJob {
      private static CameraFan? Fan1;
      private static CameraFan? Fan2;
      public async Task Execute(IJobExecutionContext context) => await StartRead(AppParams.Fan1GpioID, AppParams.Fan2GpioID);

      private static void Initialize(int fanGpioId1, int fanGpioId2) {
         if (null == Fan1) Fan1 = new(fanGpioId1);
         if (null == Fan2) Fan2 = new(fanGpioId2);
      }

      public static async Task StartRead(int gpioId1, int gpioId2) {
         try {
            Initialize(gpioId1, gpioId2);
            using CpuTemperature cpuTemperature = new();
            if (cpuTemperature.IsAvailable) {
               CpuTemperatureDatas current = new(cpuTemperature.ReadTemperatures().First().Temperature.DegreesCelsius, 0);
               DataHisto.Instance.AddCpuTempDatas(current);
               if (current.HeatingRequired) {
                  if (null != Fan1 && !Fan1.IsCooling) Fan1.StartFan(50);
                  if (null != Fan2 && !Fan2.IsCooling) Fan2.StartFan(50);
               } else {
                  if (null != Fan1 && Fan1.IsCooling) Fan1.StopFan();
                  if (null != Fan2 && Fan2.IsCooling) Fan2.StopFan();
               }
            }
         } catch (Exception ex) {
            Log.Logger.Error(ex, "[CpuTempMonitor] Error while retrieving Cpu temperature Datas");
         }
         await Task.Delay(1000);
      }
   }
}
