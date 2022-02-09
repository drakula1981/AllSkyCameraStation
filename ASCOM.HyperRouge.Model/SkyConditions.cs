using Newtonsoft.Json;
using System;

namespace ASCOM.HyperRouge.Model {
   [JsonObject("SkyConditions")]
   public class SkyConditions {
      [JsonProperty("id")] public Guid Id { get; set; }
      [JsonProperty("timeStamp")] public DateTime MeasureDate { get; private set; }
      [JsonProperty("visibleLight")] public double VisibleLight { get; private set; }
      [JsonProperty("infrared")] public double Infrared { get; private set; }
      [JsonProperty("fullSpectrum")] public double FullSpectrum { get; private set; }
      [JsonProperty("gain")] public uint Gain { get; private set; }
      [JsonProperty("integrationTime")] public uint IntegrationTime { get; private set; }
      [JsonProperty("integrated")] public double Integrated { get; private set; }
      [JsonProperty("mpsas")] public double Mpsas { get; private set; }
      [JsonProperty("dmpsas")] public double Dmpsas { get; private set; }
      public override string ToString() => JsonConvert.SerializeObject(this);
   }
}
