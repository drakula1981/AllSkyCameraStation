using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ASCOM.HyperRouge.Model {
   [JsonObject("CloudTemperatureDatas")]
   public class CloudTemperatureDatas {
      [JsonProperty("id")] public Guid Id { get; set; }
      [JsonProperty("timeStamp")] public DateTime MeasureDate { get; private set; }
      [JsonProperty("ambTemp")] public double? AmbientTemperature { get; private set; }
      [JsonProperty("sensorAmbTemp")] public double MlxAmbientTemperature { get; private set; }
      [JsonProperty("skyTemp")] public double SkyTemperature { get; private set; }
      [JsonProperty("correctionSkyTempCoeff")] public double CorSkyTemperatureCoef { get; private set; }
      [JsonProperty("correctedSkyTemp")] public double CorSkyTemperature { get; private set; }
      [JsonProperty("cloudCover")] public double CloudCoveragePercent { get; private set; }
      [JsonProperty("isSafe")] public bool IsSafe { get; private set; }

      public override string ToString() => JsonConvert.SerializeObject(this);
   }
}
