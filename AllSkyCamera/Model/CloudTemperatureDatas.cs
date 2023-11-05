using Newtonsoft.Json;
using Serilog;

namespace AllSkyCameraConditionService.Model;
[JsonObject("CloudTemperatureDatas")]
public class CloudTemperatureDatas {
   private readonly IDictionary<int, int[]> YearTempValues = new Dictionary<int, int[]>() {
         {1, new int[] { 0, -5 } },
         {2, new int[] { 0, -5 } },
         {3, new int[] { 5,  0 } },
         {4, new int[] { 0, -5 } }
      };

   private static readonly IDictionary<int, double> YearK1Values = new Dictionary<int, double>() {
         {01, 33},
         {02, 33},
         {03, 33},
         {04, 55},
         {05, 55},
         {06, 66},
         {07, 66},
         {08, 66},
         {09, 66},
         {10, 55},
         {11, 33},
         {12, 33}
      };

   private const double K2 = 0;
   private const double K3 = 4;
   private const double K4 = 100;
   private const double K5 = 100;
   private const int acceptable_cloud = 30;
   [JsonProperty("id")] public Guid Id { get; set; }
   [JsonProperty("timeStamp")] public DateTime MeasureDate { get; private set; }
   [JsonProperty("ambTemp")] public double? AmbientTemperature { get; private set; }
   [JsonProperty("sensorAmbTemp")] public double MlxAmbientTemperature { get; private set; }
   [JsonProperty("skyTemp")] public double SkyTemperature { get; private set; }
   [JsonProperty("correctionSkyTempCoeff")] public double CorSkyTemperatureCoef { get; private set; }
   [JsonProperty("correctedSkyTemp")] public double CorSkyTemperature => Math.Round(SkyTemperature - CorSkyTemperatureCoef, 2); // Temperature of the Sky after correction
   [JsonProperty("cloudCover")] public double CloudCoveragePercent { get; private set; }
   [JsonProperty("isSafe")] public bool IsSafe => CloudCoveragePercent <= acceptable_cloud;

   public CloudTemperatureDatas() : this(null, 0, 0) { }
   public CloudTemperatureDatas(DateTime measureDate, double? ambTemp, double sAmbTemp, double skyTemp) {
      Id = Guid.NewGuid();
      MeasureDate = measureDate;
      AmbientTemperature = ambTemp;
      MlxAmbientTemperature = sAmbTemp;
      SkyTemperature = skyTemp;
      CorSkyTemperatureCoef = ComputeTempCorrectionCoefficient(AmbientTemperature ?? MlxAmbientTemperature, SkyTemperature);
      CloudCoveragePercent = ComputeCloudCoveragePercent();
   }
   public CloudTemperatureDatas(double? ambTemp, double sAmbTemp, double skyTemp) {
      Id = Guid.NewGuid();
      MeasureDate = DateTime.UtcNow;
      AmbientTemperature = ambTemp;
      MlxAmbientTemperature = sAmbTemp;
      SkyTemperature = skyTemp;
      CorSkyTemperatureCoef = ComputeTempCorrectionCoefficient(AmbientTemperature ?? MlxAmbientTemperature, SkyTemperature);
      CloudCoveragePercent = ComputeCloudCoveragePercent();
   }

   private static double Map(double x, double in_min, double in_max, double out_min, double out_max) {
      var m1 = x - in_min;
      var m2 = out_max - out_min;
      var m3 = in_max - in_min;
      var m4 = out_min;
      var map = Math.Ceiling(m1 * m2 / m3 + m4);

      Log.Logger.Debug($"[CloudTemperatureDatas.Map] m1(x - in_min)[{m1}] * m2(out_max - out_min)[{m2}] / m3(in_max - in_min)[{m3}] + m4(out_min)[{m4}] => [{map}]");
      return map;
   }
   private static int GetQuarter(DateTime date) => (date.Month + 2) / 3;
   private static double ComputeTempCorrectionCoefficient(double ambiant, double skyTemp) {
      double deltaTemp = (skyTemp / ambiant) * 10;

      double K1 = YearK1Values[DateTime.Now.Month];
      Log.Logger.Debug($"[CloudTemperatureDatas.ComputeTempCorrectionCoefficient] Constants:K1[{K1}] / K2[{K2}] / K3[{K3}] / K4[{K4}] / K5[{K5}]");

      double k1 = K1 / 100;
      Log.Logger.Debug($"[CloudTemperatureDatas.ComputeTempCorrectionCoefficient] k1 = K1 / 100 ==> [{k1}]");

      double k2 = ambiant - (K2 + deltaTemp) / 10;
      Log.Logger.Debug($"[CloudTemperatureDatas.ComputeTempCorrectionCoefficient] k2 = ambiant - (K2 + deltaTemp) / 10 ==> [{k2}]");

      double k3 = K3 / 100;
      Log.Logger.Debug($"[CloudTemperatureDatas.ComputeTempCorrectionCoefficient] k3 = K3 / 100 ==> [{k3}]");

      double k4 = Math.Pow(Math.Exp(K4 / 1000 * ambiant), K5 / 100);
      Log.Logger.Debug($"[CloudTemperatureDatas.ComputeTempCorrectionCoefficient] k4 = Math.Pow(Math.Exp(K4 / 1000 * ambiant), K5 / 100) ==> [{k4}]");

      double coef = Math.Round(k1 * k2 + k3 * k4, 2);
      Log.Logger.Debug($"[CloudTemperatureDatas.ComputeTempCorrectionCoefficient] coef = Math.Round(k1 * k2 + k3 * k4, 2) ==> [{coef}]");
      return coef;
   }

   private double ComputeCloudCoveragePercent() {
      int[] quarterTempValues = YearTempValues[GetQuarter(DateTime.Now)];
      int temp_couvert = quarterTempValues[0];
      int temp_clair = quarterTempValues[1];
      double map = Map(CorSkyTemperature, temp_clair, temp_couvert, 0, 100);

      if (map < 0) return 0;
      else return (double)(map > 100 ? 100 : map);      
   }
   public override string ToString() => JsonConvert.SerializeObject(this);
   public string ToCsv() => $"{MeasureDate};{AmbientTemperature};{SkyTemperature};{IsSafe};";
}