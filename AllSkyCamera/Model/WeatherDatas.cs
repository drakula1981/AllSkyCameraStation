using Newtonsoft.Json;

namespace AllSkyCameraConditionService.Model {
   [JsonObject("WeatherDatas")]
   public class WeatherDatas {
      [JsonProperty("id")] public Guid Id { get; set; }
      [JsonProperty("timeStamp")] public DateTime MeasureDate { get; private set; }
      [JsonProperty("temp")] public double Temperature { get; private set; }
      [JsonProperty("hum")] public double Humidity { get; private set; }
      [JsonProperty("pressure")] public double Pressure { get; private set; }
      [JsonProperty("dewPoint")] public double DewPoint {
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
      public WeatherDatas() : this(0,0,0){ }

      public WeatherDatas(double temp, double hum, double pres) {
         Id = Guid.NewGuid();
         MeasureDate =DateTime.UtcNow;
         Temperature = temp;
         Humidity = hum;
         Pressure = pres;
      }
      public override string ToString() => JsonConvert.SerializeObject(this);

      public string ToCsv() => $"{MeasureDate};{Pressure};{Temperature};{Humidity};{DewPoint};{TempIndex:n0}";
   }
}
