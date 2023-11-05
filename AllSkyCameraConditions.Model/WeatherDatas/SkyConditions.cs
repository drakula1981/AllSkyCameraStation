using AllSkyCameraConditions.Model;
using Newtonsoft.Json;

namespace AllSkyCameraConditionService.Model.WeatherDatas;
internal readonly struct TemperatureCalibration {
   public static readonly float fullLuminositySlope = 0.000705244123F;
   public static readonly float fullLuminosityIntercept = 0.9794303797F;
   public static readonly float irSlope = -0.001939421338F;
   public static readonly float irIntercept = 1.05F;
}

[JsonObject("SkyConditions")]
public class SkyConditions : EntityBase {
   private const double MPSAS_CORRECTION_COEF = 1;
   [JsonProperty("visibleLight")] public double VisibleLight { get; init; }
   [JsonProperty("infrared")] public double Infrared { get; init; }
   [JsonProperty("fullSpectrum")] public double FullSpectrum { get; init; }
   [JsonProperty("gain")] public int Gain { get; init; }
   [JsonProperty("integrationTime")] public int IntegrationTime { get; init; }
   [JsonProperty("integrated")] public double Integrated { get; init; }
   [JsonProperty("mpsas")] public double Mpsas { get; init; }
   [JsonProperty("dmpsas")] public double Dmpsas => Math.Round(1.086 / Math.Sqrt((float)VisibleLight), 5);
   public SkyConditions() : this(0, 0, 0, 0, 0, 0, 0) { }
   public SkyConditions(DateTime measureDate, double vis, double ir, double fs, double inte, int gain, int intTime, double sqmCorrectionCoef) {
      Id = Guid.NewGuid();
      MeasureDate = measureDate;
      VisibleLight = vis;
      Infrared = ir;
      FullSpectrum = fs;
      Integrated = inte;
      Gain = gain;
      IntegrationTime = intTime;
      Mpsas = ComputeMpsas(vis, gain, intTime, sqmCorrectionCoef);
   }
   public SkyConditions(double vis, double ir, double fs, double inte, int gain, int intTime, double sqmCorrectionCoef) {
      Id = Guid.NewGuid();
      MeasureDate = DateTime.UtcNow;
      VisibleLight = vis;
      Infrared = ir;
      FullSpectrum = fs;
      Integrated = inte;
      Gain = gain;
      IntegrationTime = intTime;
      Mpsas = ComputeMpsas(vis, gain, intTime, sqmCorrectionCoef);
   }
   private static double ComputeMpsas(double visible, int gain, int intTime, double sqmCorrectionCoef) {
      double vis = visible / (gain * intTime / 200 * MPSAS_CORRECTION_COEF);
      double pre = 12.6 - 1.086 * Math.Log10(vis);

      return Math.Round(pre / sqmCorrectionCoef, 2);
   }
   public override string ToString() => JsonConvert.SerializeObject(this);

   public string ToCsv() => $"{MeasureDate};{FullSpectrum};{Infrared};{VisibleLight};{Integrated};{Gain};{IntegrationTime};{Mpsas};{Dmpsas}";
}

