using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Serilog;

namespace AllSkyCameraConditionService.GpioManaging {
   public class WeatherDatas(double temperature, double humidity, double pressure) {
      public double Temperature { get; init; } = temperature;
      public double Humidity { get; init; } = humidity;
      public double Pressure { get; init; } = pressure;

   }
   public class ThermalSensor(double temperatureCorrectionCoef, double humidityCorrectionCoef, double pressureCorrectionCoef) : I2CDeviceBase {
      private static int MeasurementTime { get; set; }
      private double TemperatureCorrectionCoef { get; set; } = temperatureCorrectionCoef;
      private double HumidityCorrectionCoef { get; set; } = humidityCorrectionCoef;
      private double PressureCorrectionCoef { get; set; } = pressureCorrectionCoef;

      public async Task<WeatherDatas?> StartRead(bool debugMode) {
         try {
            Initialize("Conditions", Bmx280Base.SecondaryI2cAddress);

            if (null == I2cDevice) return null;
            using Bme280 sensor = new(I2cDevice);
            if (null == sensor) {
               Log.Logger.Error($"[Conditions] Could not read weather datas due to sensor non configuration");
               return null;
            }
            sensor.SetPowerMode(Bmx280PowerMode.Forced);
            MeasurementTime = sensor.GetMeasurementDuration();
            await Task.Delay(MeasurementTime);
            var weatherDatas = await sensor.ReadAsync();
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
