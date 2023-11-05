using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Serilog;
using System.Device.I2c;

namespace AllSkyCameraConditionService.GpioManaging {
   public class WeatherDatas(double temperature, double humidity, double pressure) {
      public double Temperature { get; init; } = temperature;
      public double Humidity { get; init; } = humidity;
      public double Pressure { get; init; } = pressure;

   }
   public class ThermalSensor(double temperatureCorrectionCoef, double humidityCorrectionCoef, double pressureCorrectionCoef) {
      private static I2cConnectionSettings? I2cSettings { get; set; }
      private static I2cDevice? I2cDevice { get; set; }
      private static Bme280? Sensor { get; set; }
      private static int MeasurementTime { get; set; }
      private static bool IsInitialized => I2cSettings != null;
      private double TemperatureCorrectionCoef { get; set; } = temperatureCorrectionCoef;
      private double HumidityCorrectionCoef { get; set; } = humidityCorrectionCoef;
      private double PressureCorrectionCoef { get; set; } = pressureCorrectionCoef;

      private static void Initialize() {
         if (IsInitialized) return;
         try {
            I2cSettings = new(1, Bmx280Base.SecondaryI2cAddress);
            I2cDevice = I2cDevice.Create(I2cSettings);
            Sensor = new(I2cDevice);
            MeasurementTime = Sensor.GetMeasurementDuration();
         } catch (IOException ioex) {
            Log.Logger.Error($"[Conditions] Error while intializing weather datas sensor : {ioex.Message}");
         } catch (Exception ex) {
            Log.Logger.Error(ex, "[Conditions] Error while intializing weather datas sensor");
         }
      }

      public async Task<WeatherDatas?> StartRead(bool debugMode) {
         try {
            Initialize();
            if (null == Sensor) {
               Log.Logger.Error($"[Conditions] Could not read weather datas due to sensor non configuration");
               return null;
            }
            Sensor.SetPowerMode(Bmx280PowerMode.Forced);
            await Task.Delay(MeasurementTime);
            var weatherDatas = await Sensor.ReadAsync();
            if (debugMode) Log.Logger.Information($"[Conditions] t={weatherDatas.Temperature.GetValueOrDefault().DegreesCelsius};h={weatherDatas.Humidity.GetValueOrDefault().Percent};p={weatherDatas.Pressure.GetValueOrDefault().Hectopascals}");
            return new WeatherDatas(weatherDatas.Temperature.HasValue ? Math.Round(weatherDatas.Temperature.Value.DegreesCelsius / TemperatureCorrectionCoef, 2) : 0,
                                  weatherDatas.Humidity.HasValue ? Math.Round(weatherDatas.Humidity.Value.Percent / HumidityCorrectionCoef, 1) : 0,
                                  weatherDatas.Pressure.HasValue ? Math.Floor(weatherDatas.Pressure.Value.Hectopascals / PressureCorrectionCoef) : 0);
         } catch (IOException ioex) {
            Log.Logger.Error(ioex.Message, "[Conditions] Error while reading Weather Datas");
            return null;
         } catch (Exception ex) {
            Log.Logger.Error(ex, "[Conditions] Error while retrieving Weather Datas");
            return null;
         }
      }
   }
}
