using AllSkyCameraConditions.Model;
using Newtonsoft.Json;

namespace AllSkyCameraConditionService.Model.WeatherDatas;
[JsonObject("CurrentConditions")]
public class CurrentConditions : EntityBase {
   [JsonProperty(nameof(Temperature))] public double? Temperature { get; set; }
   [JsonProperty(nameof(Humidity))] public double? Humidity { get; set; }
   [JsonProperty(nameof(Pressure))] public double? Pressure { get; set; }
   [JsonProperty(nameof(DewPoint))] public double? DewPoint { get; set; }
   [JsonProperty(nameof(SkyTemperature))] public double? SkyTemperature { get; set; }
   [JsonProperty(nameof(CloudCoverage))] public double? CloudCoverage { get; set; }
   [JsonProperty(nameof(SkyBrightness))] public double? SkyBrightness { get; set; }
   [JsonProperty(nameof(SkyQuality))] public double? SkyQuality { get; set; }
   [JsonProperty(nameof(IsSafe))] public bool? IsSafe { get; set; }
   [JsonProperty(nameof(MoonAge))] public string? MoonAge { get; set; }
   public override string ToString() => JsonConvert.SerializeObject(this);
}
