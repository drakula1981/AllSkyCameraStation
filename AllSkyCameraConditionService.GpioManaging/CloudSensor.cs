using Iot.Device.Mlx90614;
using Serilog;
using System.Device.I2c;

namespace AllSkyCameraConditionService.GpioManaging;
public class CloudTemperatureDatas(double currentTemp, double ambientTemp, double sensorTemp) {
   public double CurrentTemperature { get; init; } = currentTemp;
   public double SensorAmbientTemperature { get; init; } = ambientTemp;
   public double SensorTemperature { get; init; } = sensorTemp;
}
public class CloudSensor {
   private static I2cConnectionSettings? I2cSettings { get; set; }
   private static I2cDevice? I2cDevice { get; set; }
   private static bool IsInitialized => I2cSettings != null;

   private static void Initialize() {
      if (IsInitialized) return;
      try {
         I2cSettings = new(1, Mlx90614.DefaultI2cAddress);
         I2cDevice = I2cDevice.Create(I2cSettings);
      } catch (IOException ioex) {
         Log.Logger.Error($"[Conditions] Error while intializing weather datas sensor : {ioex.Message}");
      } catch (Exception ex) {
         Log.Logger.Error(ex, "[Conditions] Error while intializing weather datas sensor");
      }
   }

   public static CloudTemperatureDatas? ReadCloudDatas(int retryNum, double currentTemp, bool debugMode) {
      try {
         Initialize();

         using Mlx90614 sensor = new(I2cDevice);
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
