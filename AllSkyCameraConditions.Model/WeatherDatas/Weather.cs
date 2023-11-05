using AllSkyCameraConditions.Model;
using Newtonsoft.Json;

namespace AllSkyCameraConditionService.Model.WeatherDatas;
[JsonObject("Weather")]
public class Weather : EntityBase {
   [JsonProperty("temp")] public double Temperature { get; init; }
   [JsonProperty("hum")] public double Humidity { get; init; }
   [JsonProperty("pressure")] public double Pressure { get; init; }
   [JsonProperty("dewPoint")]
   public double DewPoint {
      get {
         double a = 7.5, b = 237.3, T = Temperature, RH = Humidity / 100;
         double inter = 6.1078 * Math.Pow(10.0, a * T / (b + T));
         double humRatio = Math.Log10(RH * inter / 6.1078);
         return Math.Round((b * humRatio) / (a - humRatio), 1);
      }
   }
   [JsonProperty("cloudHeight")] public double CloudHeight => Math.Round((Temperature - DewPoint) * 125.0, 0);
   [JsonProperty("heatIndex")] public double HeatIndex => Math.Round(1.61139411 * Temperature - 8.784695 + 2.338549 * Humidity - 0.14611605 * Temperature * Humidity - 0.012308094 * Math.Pow(Temperature, 2.0) - 0.016424828 * Math.Pow(Humidity, 2.0) + 0.002211732 * Math.Pow(Temperature, 2.0) * Humidity + 0.00072546 * Temperature * Math.Pow(Humidity, 2.0) - 3.582E-06 * Math.Pow(Temperature, 2.0) * Math.Pow(Humidity, 2.0), 1);
   [JsonProperty("tempIndex")] public double TempIndex => Math.Round(Temperature / DewPoint, 0);
   public Weather() : this(0, 0, 0) { }
   public Weather(DateTime measureDate, double temp, double hum, double pres) {
      Id = Guid.NewGuid();
      MeasureDate = measureDate;
      Temperature = temp;
      Humidity = hum > 100 ? 100 : hum;
      Pressure = pres;
   }
   public Weather(double temp, double hum, double pres) : this(DateTime.UtcNow, temp, hum, pres) { }

   public override string ToString() => JsonConvert.SerializeObject(this);

   public string ToCsv() => $"{MeasureDate};{Pressure};{Temperature};{Humidity};{DewPoint};{TempIndex:n0}";
}