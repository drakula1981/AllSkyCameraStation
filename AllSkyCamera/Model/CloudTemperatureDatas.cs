using Newtonsoft.Json;
using Serilog;

namespace AllSkyCameraConditionService.Model {
   [JsonObject("CloudTemperatureDatas")]
   public class CloudTemperatureDatas {
      private readonly IDictionary<int, int[]> YearTempValues = new Dictionary<int, int[]>() {
         {1, new int[] { 0, -5 } },
         {2, new int[] { 0, -5 } },
         {3, new int[] { 5,  0 } },
         {4, new int[] { 0, -5 } }
      };
      private const double K1_Winter = 33;
      private const double K1_Summer = 66;
      private const double K1_HighTemp = 80;

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
         //Log.Logger.Information($"[CloudTemperatureDatas.Map] m1(x - in_min)[{m1}] * m2(out_max - out_min)[{m2}] / m3(in_max - in_min)[{m3}] + m4(out_min)[{m4}] => [{m1 * m2 / m3 + m4}]");
         return Math.Ceiling(m1 * m2 / m3 + m4);
      }
      private static int GetQuarter(DateTime date) => (date.Month + 2) / 3;
      private static double ComputeTempCorrectionCoefficient(double ambiant, double skyTemp) {
         double deltaTemp = (skyTemp / ambiant)*10;
         double k1 = (GetQuarter(DateTime.Now) == 3 ? deltaTemp <= 7 || DateTime.Now.Hour <= 5 || DateTime.Now.Hour >= 21 ? K1_Summer: deltaTemp >= 8 ? K1_HighTemp + 10 : K1_HighTemp : K1_Winter) / 100;
         double k2 = ambiant - (K2 + deltaTemp) / 10;
         double k3 = K3 / 100;
         double k4 = Math.Pow(Math.Exp(K4 / 1000 * ambiant), K5 / 100);
         //Log.Logger.Information($"[CloudTemperatureDatas.ComputeTempCorrectionCoefficient] k1[{k1}] * k2[{k2}] + k3[{k3}] * k4 [{k4}] => {Math.Round(k1 * k2 + k3 * k4, 2)}");
         return Math.Round(k1 * k2 + k3 * k4, 2);
      }

      private double ComputeCloudCoveragePercent() {
         int[] quarterTempValues = YearTempValues[GetQuarter(DateTime.Now)];
         int temp_couvert = quarterTempValues[0];
         int temp_clair = quarterTempValues[1];
         //double delta = CorSkyTemperature;
         double map = Map(CorSkyTemperature, temp_clair, temp_couvert, 0, 100);
         //Log.Logger.Information($"[CloudTemperatureDatas.ComputeCloudCoveragePercent] (delta[{delta}], temp_clair[{temp_clair}], temp_couvert[{temp_couvert}] => map[{(map < 0 ? 0 : map > 100 ? 100 : map)}]");

         return map < 0 ? 0 : map > 100 ? 100 : map;
      }
      public override string ToString() => JsonConvert.SerializeObject(this);
      public string ToCsv() => $"{MeasureDate};{AmbientTemperature};{SkyTemperature};{IsSafe};";
   }
}