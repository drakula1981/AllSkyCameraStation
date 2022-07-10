using Newtonsoft.Json;
using Serilog;
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
      private const double MPSAS_CORRECTION_COEF = 1;
      [JsonProperty("id")] public Guid Id { get; set; }
      [JsonProperty("timeStamp")] public DateTime MeasureDate { get; private set; }
      [JsonProperty("visibleLight")] public double VisibleLight { get; private set; }
      [JsonProperty("infrared")] public double Infrared { get; private set; }
      [JsonProperty("fullSpectrum")] public double FullSpectrum { get; private set; }
      [JsonProperty("gain")] public int Gain { get; private set; }
      [JsonProperty("integrationTime")] public int IntegrationTime { get; private set; }
      [JsonProperty("integrated")] public double Integrated { get; private set; }
      [JsonProperty("mpsas")] public double Mpsas { get; private set; }
      [JsonProperty("dmpsas")] public double Dmpsas => Math.Round(1.086 / Math.Sqrt((float)VisibleLight), 5);
      public SkyConditions() : this(0, 0, 0, 0, 0, 0) { }
      public SkyConditions(DateTime measureDate, double vis, double ir, double fs, double inte, int gain, int intTime) {
         Id = Guid.NewGuid();
         MeasureDate = measureDate;
         VisibleLight = vis;
         Infrared = ir;
         FullSpectrum = fs;
         Integrated = inte;
         Gain = gain;
         IntegrationTime = intTime;
         Mpsas = ComputeMpsas(vis, gain, intTime);
      }
      public SkyConditions(double vis, double ir, double fs, double inte, int gain, int intTime) {
         Id = Guid.NewGuid();
         MeasureDate = DateTime.UtcNow;
         VisibleLight = vis;
         Infrared = ir;
         FullSpectrum = fs;
         Integrated = inte;
         Gain = gain;
         IntegrationTime = intTime;
         Mpsas = ComputeMpsas(vis, gain, intTime);
      }
      private static double ComputeMpsas(double visible, int gain, int intTime) {
         double vis = visible / (gain * intTime / 200 * MPSAS_CORRECTION_COEF);
         double pre = 12.6 - 1.086 * Math.Log10(vis);
         /*Log.Information($"ComputeMpsas => vis                         = {vis}");
         Log.Information($"ComputeMpsas => AppParams.SqmCorrectionCoef = {AppParams.SqmCorrectionCoef}");
         Log.Information($"ComputeMpsas => precalcul                   = {pre}");*/
         return Math.Round(pre / AppParams.SqmCorrectionCoef, 2);
      }
      public override string ToString() => JsonConvert.SerializeObject(this);

      public string ToCsv() => $"{MeasureDate};{FullSpectrum};{Infrared};{VisibleLight};{Integrated};{Gain};{IntegrationTime};{Mpsas};{Dmpsas}";
   }
}
