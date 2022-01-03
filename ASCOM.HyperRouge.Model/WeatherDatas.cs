using Newtonsoft.Json;
using System;

namespace ASCOM.HyperRouge.Model {
   [JsonObject("WeatherDatas")]
   public class WeatherDatas {
      [JsonProperty("id")] public Guid Id { get; set; }
      [JsonProperty("timeStamp")] public DateTime MeasureDate { get; private set; }
      [JsonProperty("temp")] public double Temperature { get; private set; }
      [JsonProperty("hum")] public double Humidity { get; private set; }
      [JsonProperty("pressure")] public double Pressure { get; private set; }
      [JsonProperty("dewPoint")] public double DewPoint { get; private set; }
      [JsonProperty("cloudHeight")] public double CloudHeight { get; private set; }
      [JsonProperty("heatIndex")] public double HeatIndex { get; private set; }
      [JsonProperty("tempIndex")] public double TempIndex { get; private set; }
      public override string ToString() => JsonConvert.SerializeObject(this);
   }
}
