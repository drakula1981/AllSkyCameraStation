using AllSkyCameraConditionService.GpioManaging;
using AllSkyCameraConditionService.Model;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Quartz;
using Serilog;
using System.Device.I2c;
using System.Text;

namespace AllSkyCameraConditionService.Jobs;
internal class Conditions : IJob {
   private static Heater? heater;
   private static I2cConnectionSettings? I2cSettings { get; set; }
   private static I2cDevice? I2cDevice { get; set; }
   private static Bme280? Sensor { get; set; }
   private static int MeasurementTime { get; set; }
   private static bool IsInitialized => I2cSettings != null;
   public static WeatherDatas? LastMeasure => DataHisto.Instance.WeatherDatasHisto.LastOrDefault();

   private static void Initialize(int gpioID) {
      if (IsInitialized) return;
      try {
         I2cSettings = new(1, Bmx280Base.SecondaryI2cAddress);
         I2cDevice = I2cDevice.Create(I2cSettings);
         Sensor = new(I2cDevice);
         MeasurementTime = Sensor.GetMeasurementDuration();
         heater = new(gpioID);
         heater.StopHeat();
      } catch (IOException ioex) {
         Log.Logger.Error($"[Conditions] Error while intializing weather datas sensor : {ioex.Message}");
      } catch (Exception ex) {
         Log.Logger.Error(ex, "[Conditions] Error while intializing weather datas sensor");
      }
   }

   public async Task Execute(IJobExecutionContext context) => await StartRead(AppParams.MaxHeatingTemp
                                                                            , AppParams.HeaterGpioID
                                                                            , AppParams.TemperatureCorrectionCoef
                                                                            , AppParams.HumidityCorrectionCoef
                                                                            , AppParams.PressureCorrectionCoef);

   public static async Task StartRead(int maxHeatingTemp, int gpioID, double tempAdj, double humAdj, double presAdj) {
      try {
         Initialize(gpioID);
         if (null == Sensor) {
            Log.Logger.Error($"[Conditions] Could not read weather datas due to sensor non configuration");
            return;
         }
         Sensor.SetPowerMode(Bmx280PowerMode.Forced);
         await Task.Delay(MeasurementTime);
         var weatherDatas = await Sensor.ReadAsync();
         if (AppParams.DebugMode) Log.Logger.Information($"[Conditions] t={weatherDatas.Temperature.GetValueOrDefault().DegreesCelsius};h={weatherDatas.Humidity.GetValueOrDefault().Percent};p={weatherDatas.Pressure.GetValueOrDefault().Hectopascals}");
         WeatherDatas wd = new(weatherDatas.Temperature.HasValue ? Math.Round(weatherDatas.Temperature.Value.DegreesCelsius / tempAdj, 2) : 0,
                               weatherDatas.Humidity.HasValue ? Math.Round(weatherDatas.Humidity.Value.Percent / humAdj, 1) : 0,
                               weatherDatas.Pressure.HasValue ? Math.Floor(weatherDatas.Pressure.Value.Hectopascals / presAdj) : 0);
         DataHisto.Instance.AddWeatherDatas(wd);
         if (AppParams.DebugMode) Log.Logger.Information($"[Conditions] {wd}");
         if (null == heater || heater.IsHeating) {
            Log.Logger.Information($"[Conditions] Currently heating...");
            return;
         }
         if ((wd.TempIndex < 3 && wd.Temperature <= maxHeatingTemp) || wd.Humidity >= 90) {
            if (AppParams.DebugMode) Log.Logger.Information($"[Conditions] Starting heating...");
            if (!heater.IsHeating) heater.Heat();
         } else {
            if (AppParams.DebugMode) Log.Logger.Information($"[Conditions] Heating conditions not reached : TempIndex : {wd.TempIndex} / CurrentTemp : {wd.Temperature} / MaxHeatingTemp : {maxHeatingTemp} ");
            heater.StopHeat();
         }
      } catch (IOException ioex) {
         Log.Logger.Error(ioex.Message, "[Conditions] Error while reading Weather Datas");
      } catch (Exception ex) {
         Log.Logger.Error(ex, "[Conditions] Error while retrieving Weather Datas");
      }
   }

   public static async Task ToCsv(FileInfo destFile) {
      StringBuilder sb = new(destFile.Exists ? $"date;temp;hum;dewpoint;tempIndex{Environment.NewLine}" : "");
      DataHisto.Instance.WeatherDatasHisto.ForEach(w => sb.AppendLine(w.ToCsv()));
      byte[] buffer = Encoding.Unicode.GetBytes(sb.ToString());
      using FileStream fs = destFile.Open(FileMode.OpenOrCreate);
      await fs.WriteAsync(buffer);
      fs.Flush();
      fs.Close();
   }
}
