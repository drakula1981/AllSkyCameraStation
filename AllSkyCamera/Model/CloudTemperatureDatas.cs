using Newtonsoft.Json;

namespace AllSkyCameraConditionService.Model {
   [JsonObject("CloudTemperatureDatas")]
   public class CloudTemperatureDatas {
      private readonly IDictionary<int, int[]> YearTempValues = new Dictionary<int, int[]>() {
         {1, new int[] {0, -5 } },
         {2, new int[] {5, 0 } },
         {3, new int[] {5, 0 } },
         {4, new int[] {0, -5 } }
      };
      private const int K1 = 33;
      private const int K2 = 0;
      private const int K3 = 4;
      private const int K4 = 100;
      private const int K5 = 100;
      private const int acceptable_cloud = 30;
      [JsonProperty("id")] public Guid Id { get; set; }
      [JsonProperty("timeStamp")] public DateTime MeasureDate { get; private set; }
      [JsonProperty("ambTemp")] public double? AmbientTemperature { get; private set; }
      [JsonProperty("sensorAmbTemp")] public double MlxAmbientTemperature { get; private set; }
      [JsonProperty("skyTemp")] public double SkyTemperature { get; private set; }
      [JsonProperty("correctionSkyTempCoeff")] public double CorSkyTemperatureCoef {
         get {
            double ambiant = AmbientTemperature ?? MlxAmbientTemperature;
            return (K1 / 100) * (ambiant - K2 / 10) + (K3 / 100) * Math.Pow((Math.Exp(K4 / 1000 * ambiant)), (K5 / 100));  // calcul of the correction of the sky temperature
         }
      }
      [JsonProperty("correctedSkyTemp")] public double CorSkyTemperature => SkyTemperature - CorSkyTemperatureCoef; // Temperature of the Sky after correction
      [JsonProperty("cloudCover")] public double CloudCoveragePercent {
         get {
            int[] quarterTempValues = YearTempValues[GetQuarter(DateTime.Now)];
            int temp_couvert = quarterTempValues[0];
            int temp_clair = quarterTempValues[1];
            double delta = CorSkyTemperature - temp_couvert;
            double map = Map(delta, temp_clair, temp_couvert, 0, 100);

            /*if (delta > temp_couvert) return 100;
            if (delta < temp_clair) return 0;
            if ((delta >= temp_clair) && (delta <= temp_couvert)) {*/
            return map < 0 ? 0 : map > 100 ? 100 : map;
            /*}
            return -1;*/
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
      }

      private static double Map(double x, double in_min, double in_max, double out_min, double out_max) => (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
      private static int GetQuarter(DateTime date) => (date.Month + 2) / 3;

      public override string ToString() => JsonConvert.SerializeObject(this);

      public string ToCsv() => $"{MeasureDate};{AmbientTemperature};{SkyTemperature};{IsSafe};";
   }
}