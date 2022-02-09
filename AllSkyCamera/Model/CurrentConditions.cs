using Newtonsoft.Json;

namespace AllSkyCameraConditionService.Model {
   [JsonObject("CurrentConditions")]
   internal class CurrentConditions {
      [JsonProperty("MeasureDate")] public DateTime? MeasureDate { get; set; }
      [JsonProperty("Temperature")] public double? Temperature { get; set; }
      [JsonProperty("Humidity")] public double? Humidity { get; set; }
      [JsonProperty("Pressure")] public double? Pressure { get; set; }
      [JsonProperty("DewPoint")] public double? DewPoint { get; set; }
      [JsonProperty("SkyTemperature")] public double? SkyTemperature { get; set; }
      [JsonProperty("SkyBrightness")] public double? SkyBrightness { get; set; }
      [JsonProperty("SkyQuality")] public double? SkyQuality { get; set; }
      [JsonProperty("CloudCoverage")] public double? CloudCoverage { get; set; }
      [JsonProperty("IsSafe")] public bool? IsSafe { get; set; }

      public override string ToString() => JsonConvert.SerializeObject(this);

   }
}
