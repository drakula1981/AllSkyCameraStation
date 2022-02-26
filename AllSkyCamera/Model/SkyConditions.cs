using Newtonsoft.Json;
using UnitsNet;

namespace AllSkyCameraConditionService.Model {
   public struct TemperatureCalibration {
      public static readonly float fullLuminositySlope = 0.000705244123F;
      public static readonly float fullLuminosityIntercept = 0.9794303797F;
      public static readonly float irSlope = -0.001939421338F;
      public static readonly float irIntercept = 1.05F;
   }

   [JsonObject("SkyConditions")]
   public class SkyConditions {
      private const float MPSAS_CORRECTION_COEF = 1F;
      [JsonProperty("id")] public Guid Id { get; set; }
      [JsonProperty("timeStamp")] public DateTime MeasureDate { get; private set; }
      [JsonProperty("visibleLight")] public double VisibleLight { get; private set; }
      [JsonProperty("infrared")] public double Infrared { get; private set; }
      [JsonProperty("fullSpectrum")] public double FullSpectrum { get; private set; }
      [JsonProperty("gain")] public int Gain { get; private set; }
      [JsonProperty("integrationTime")] public int IntegrationTime { get; private set; }
      [JsonProperty("integrated")] public double Integrated { get; private set; }
      [JsonProperty("mpsas")] public double Mpsas { 
         get {
            float vis = (float)VisibleLight / (Gain * IntegrationTime / 200F * MPSAS_CORRECTION_COEF);
            return Math.Round((12.6 - 1.086 * Math.Log(vis)) / AppParams.SqmCorrectionCoef, 2);
         } 
      }
      [JsonProperty("dmpsas")] public double Dmpsas => Math.Round(1.086 / Math.Sqrt((float)VisibleLight),5);
      public SkyConditions() : this(0, 0, 0, 0, 0, 0) { }

      public SkyConditions(double vis, double ir, double fs, double inte, int gain, int intTime) {
         Id = Guid.NewGuid();
         MeasureDate = DateTime.UtcNow;
         VisibleLight = vis;
         Infrared = ir;
         FullSpectrum = fs;
         Integrated = inte;
         Gain = gain;
         IntegrationTime = intTime;
      }
      public override string ToString() => JsonConvert.SerializeObject(this);

      public string ToCsv() => $"{MeasureDate};{FullSpectrum};{Infrared};{VisibleLight};{Integrated};{Gain};{IntegrationTime};{Mpsas};{Dmpsas}";
   }
}
