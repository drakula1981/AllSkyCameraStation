using Newtonsoft.Json;
using UnitsNet;

namespace AllSkyCameraConditionService.Model {
   [JsonObject("Illuminance")]
   public class SkyConditions {
      [JsonProperty("id")] public Guid Id { get; set; }
      [JsonProperty("timeStamp")] public DateTime MeasureDate { get; private set; }
      [JsonProperty("visibleLight")] public Illuminance? VisibleLight { get; private set; }
      [JsonProperty("infrared")] public Illuminance? Infrared { get; private set; }
      [JsonProperty("fullSpectrum")] public Illuminance? FullSpectrum { get; private set; }
      [JsonProperty("fullSpectrum")] public Illuminance? Integrated { get; private set; }
      [JsonProperty("mpsas")] public double? Mpsas { 
         get {
            float vis = VisibleLight.HasValue ? (float)VisibleLight?.Value / (9876.0F * 500F / 200F * 1) : 0;
            return 12.6 - 1.086 * Math.Log(vis);
         } 
      }
      [JsonProperty("dmpsas")] public double? Dmpsas  => VisibleLight.HasValue ? 1.086 / Math.Sqrt((float)VisibleLight?.Value) : 0;
      public SkyConditions() : this(0, 0, 0, 0) { }

      public SkyConditions(double vis, double ir, double fs, double inte) {
         Id = Guid.NewGuid();
         MeasureDate = DateTime.UtcNow;
         VisibleLight = new(vis, UnitsNet.Units.IlluminanceUnit.Lux);
         Infrared = new(ir, UnitsNet.Units.IlluminanceUnit.Lux);
         FullSpectrum = new(fs, UnitsNet.Units.IlluminanceUnit.Lux);
         Integrated = new(inte, UnitsNet.Units.IlluminanceUnit.Lux);
      }
      public override string ToString() => JsonConvert.SerializeObject(this);

      public string ToCsv() => $"{MeasureDate};{FullSpectrum?.Value};{Infrared?.Value};{VisibleLight?.Value};{Integrated?.Value};{Mpsas};{Dmpsas}";
   }
}
