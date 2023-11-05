using Iot.Device.Mlx90614;
using Serilog;
using System.Device.I2c;

namespace AllSkyCameraConditionService.GpioManaging;
public class CloudTemperatureDatas(double currentTemp, double ambientTemp, double sensorTemp)  {
   public double CurrentTemperature { get; init; } = currentTemp;
   public double SensorAmbientTemperature { get; init; } = ambientTemp;
   public double SensorTemperature { get; init; } = sensorTemp;
}
public class CloudSensor : I2CDeviceBase {
   public static CloudTemperatureDatas? ReadCloudDatas(int retryNum, double currentTemp, bool debugMode) {
      try {
         Initialize("CloudSensor", Mlx90614.DefaultI2cAddress);
         if (null == I2cDevice) return null;
         using Mlx90614 sensor = new(I2cDevice);
         if (null == sensor) {
            Log.Logger.Error($"[CloudSensor] Could not read cloud datas due to sensor non configuration");
            return null;
         }
         CloudTemperatureDatas current = new(currentTemp, sensor.ReadAmbientTemperature().DegreesCelsius, sensor.ReadObjectTemperature().DegreesCelsius);
         if (debugMode) Log.Logger.Information($"[CloudSensor] {current}");
         return current;
      } catch (IOException ioex) {
         Log.Logger.Error($"[CloudSensor] Error while retrieving Cloud Datas ({(retryNum + 1).Ordinal()} attempt) : {ioex.Message}");
         if (retryNum <= 5) {
            Task.Delay(1000);
            return ReadCloudDatas(retryNum + 1, currentTemp, debugMode);
         } else return null;
      } catch (Exception ex) {
         Log.Logger.Error(ex, $"[CloudSensor] Error while retrieving Cloud Datas ({(retryNum + 1).Ordinal()} attempt)");
         return null;
      }

   }
}
