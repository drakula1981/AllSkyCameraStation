using AllSkyCameraConditionService.Model;
using Iot.Device.Mlx90614;
using Quartz;
using Serilog;
using System.Device.I2c;
using System.Text;

namespace AllSkyCameraConditionService.Jobs;
internal class CloudSensor : IJob {
   public static CloudTemperatureDatas? LastMeasure => DataHisto.Instance.CloudWatchHisto.LastOrDefault();
   public async Task Execute(IJobExecutionContext context) => await ReadTemperature();
   public static async Task ReadTemperature() => await ReadTemperature(0);
   public static async Task ReadTemperature(int retryNum) {
      try {
         I2cConnectionSettings settings = new(1, Mlx90614.DefaultI2cAddress);
         I2cDevice i2cDevice = I2cDevice.Create(settings);

         using Mlx90614 sensor = new(i2cDevice);
         double? currrentTemp = (DataHisto.Instance.LastWeatherDatas ?? new WeatherDatas()).Temperature;
         CloudTemperatureDatas current = new(currrentTemp, sensor.ReadAmbientTemperature().DegreesCelsius, sensor.ReadObjectTemperature().DegreesCelsius);
         DataHisto.Instance.AddCloudWatcherDatas(current);
         if (AppParams.DebugMode) Log.Logger.Information($"[CloudSensor] {current}");
      } catch (IOException ioex) {
         Log.Logger.Error($"[CloudSensor] Error while retrieving Cloud Datas ({(retryNum + 1).Ordinal()} attempt) : {ioex.Message}");
         if (retryNum <= 5) {
            await Task.Delay(1000);
            await ReadTemperature(retryNum + 1);
         }
      } catch (Exception ex) {
         Log.Logger.Error(ex, $"[CloudSensor] Error while retrieving Cloud Datas ({(retryNum + 1).Ordinal()} attempt)");
      }
      await Task.Delay(1000);
   }

   public static async Task ToCsv(FileInfo destFile) {
      StringBuilder sb = new(destFile.Exists ? $"date;ambTemp;skyTemp;safe{Environment.NewLine}" : "");
      DataHisto.Instance.CloudWatchHisto.ForEach(w => sb.AppendLine(w.ToCsv()));
      byte[] buffer = Encoding.Unicode.GetBytes(sb.ToString());
      using FileStream fs = destFile.Open(FileMode.OpenOrCreate);
      await fs.WriteAsync(buffer);
      fs.Flush();
      fs.Close();
   }
}
