using Newtonsoft.Json;

namespace AllSkyCameraConditionService.Model {
   [JsonObject("CurrentConditions")]
   internal class CurrentConditions {
      [JsonProperty("TimeStamp")] public DateTime? MeasureDate { get; set; }
      [JsonProperty("Temp")] public double? Temperature { get; set; }
      [JsonProperty("Hum")] public double? Humidity { get; set; }
      [JsonProperty("Pressure")] public double? Pressure { get; set; }
      [JsonProperty("DewPoint")] public double? DewPoint { get; set; }
      [JsonProperty("SkyTemp")] public double? SkyTemperature { get; set; }
      [JsonProperty("SkyBrightness")] public double? SkyBrightness { get; set; }
      [JsonProperty("SkyQuality")] public double? SkyQuality { get; set; }
      [JsonProperty("CloudCover")] public double? CloudCoveragePercent { get; set; }
      [JsonProperty("IsSafe")] public bool? IsSafe { get; set; }

      public override string ToString() => JsonConvert.SerializeObject(this);

   }
}
