using AllSkyCameraConditions.Interfaces.WeatherDatas.CloudCoverage;
using AllSkyCameraConditionService.GpioManaging;
using AllSkyCameraConditionService.Model.WeatherDatas;

namespace AllSkyCameraConditions.Services.WeatherDatas.CloudCoverage {
   internal class CloudCoverageMeasureService : ICloudCoverageMeasureService {
      public Task<CloudCoverture?> ReadMeasure(double? measureContext1 = null, double? measureContext2 = null, double? measureContext3 = null, double? measureContext4 = null, double? measureContext5 = null) {
         throw new NotImplementedException();
      }

      public CloudCoverture ReadMeasure(string? measureContext = null) => throw new NotImplementedException();

      public CloudCoverture? ReadMeasure(int? measureContext1 = null, double? measureContext2 = null, bool debugMode = false) {
         var csd = CloudSensor.ReadCloudDatas(measureContext1 ?? 1, measureContext2 ?? 1, debugMode);
         return null != csd ? new CloudCoverture(csd.CurrentTemperature, csd.SensorAmbientTemperature, csd.SensorTemperature) : null;
      }
   }
}
