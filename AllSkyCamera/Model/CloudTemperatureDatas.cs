using Newtonsoft.Json;
using Serilog;

namespace AllSkyCameraConditionService.Model {
   [JsonObject("CloudTemperatureDatas")]
   public class CloudTemperatureDatas {
      private readonly IDictionary<int, int[]> YearTempValues = new Dictionary<int, int[]>() {
         {1, new int[] {0, -5 } },
         {2, new int[] {5, 0 } },
         {3, new int[] {15, 10 } },
         {4, new int[] {0, -5 } }
      };
      private const double K1 = 66;
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
      [JsonProperty("correctedSkyTemp")] public double CorSkyTemperature => (SkyTemperature * AppParams.CloudTemperatureCoef) - CorSkyTemperatureCoef; // Temperature of the Sky after correction
      [JsonProperty("cloudCover")] public double CloudCoveragePercent {
         get {
            int[] quarterTempValues = YearTempValues[GetQuarter(DateTime.Now)];
            int temp_couvert = quarterTempValues[0];
            int temp_clair = quarterTempValues[1];
            double delta = CorSkyTemperature - temp_couvert;
            double map = Map(delta, temp_clair, temp_couvert, 0, 100);

            return map < 0 ? 0 : map > 100 ? 100 : map;
         }
      }
      [JsonProperty("isSafe")] public bool IsSafe => CloudCoveragePercent <= acceptable_cloud;

      public CloudTemperatureDatas() : this(null, 0, 0) { }
      public CloudTemperatureDatas(double? ambTemp, double sAmbTemp, double skyTemp) {
         Id = Guid.NewGuid();
         MeasureDate = DateTime.UtcNow;
         AmbientTemperature = ambTemp;
         MlxAmbientTemperature = sAmbTemp;
         SkyTemperature = skyTemp;
         CorSkyTemperatureCoef = ComputeTempCorrectionCoefficient((AmbientTemperature ?? MlxAmbientTemperature));
      }

      private static double Map(double x, double in_min, double in_max, double out_min, double out_max) => (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
      private static int GetQuarter(DateTime date) => (date.Month + 2) / 3;
      private static double ComputeTempCorrectionCoefficient(double ambiant) {
         double k1 = K1 / 100;
         double k2 = ambiant - K2 / 10;
         double k3 = K3 / 100;
         double k4 = Math.Pow(Math.Exp(K4 / 1000 * ambiant), K5 / 100);
         //Log.Logger.Information($"[CloudTemperatureDatas.ComputeTempCorrectionCoefficient] k1 = {k1} * k2 = {k2} + k3 = {k3} * k4 = {k4} => {k1 * k2 + k3 * k4}");
         return k1 * k2 + k3 * k4;
      }
      public override string ToString() => JsonConvert.SerializeObject(this);
      public string ToCsv() => $"{MeasureDate};{AmbientTemperature};{SkyTemperature};{IsSafe};";
   }
}