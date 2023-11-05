using AllSkyCameraConditionService.Model;
using Iot.Device.CpuTemperature;
using Quartz;
using Serilog;

namespace AllSkyCameraConditionService.Jobs;
internal class CpuTempMonitor : IJob {
   public async Task Execute(IJobExecutionContext context) => await StartRead();

   public static async Task StartRead() {
      try {
         using CpuTemperature cpuTemperature = new();
         Log.Logger.Debug("[CpuTempMonitor] Initializing Cpu temperature Monitor");

         if (cpuTemperature.IsAvailable) {
            CpuTemperatureDatas current = new(Math.Round(cpuTemperature.ReadTemperatures().First().Temperature.DegreesCelsius, 1));
            Log.Logger.Information($"[CpuTempMonitor] {current}");
            DataHisto.Instance.AddCpuTempDatas(current);
         }
      } catch (Exception ex) {
         Log.Logger.Error(ex, "[CpuTempMonitor] Error while retrieving Cpu temperature Datas");
      }
      await Task.Delay(1000);
   }
}
