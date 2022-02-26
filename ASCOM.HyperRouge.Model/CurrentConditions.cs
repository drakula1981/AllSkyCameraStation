using Newtonsoft.Json;
using System;
using System.Net;

namespace ASCOM.HyperRouge.Model {
   [JsonObject("CurrentConditions")]
   public class CurrentConditions {
      private const double REFRESH_TIMEOUT = 1;
      [JsonProperty("MeasureDate")] public DateTime? MeasureDate { get; set; }
      [JsonProperty("Temperature")] public double? Temperature { get; set; }
      [JsonProperty("Humidity")] public double? Humidity { get; set; }
      [JsonProperty("Pressure")] public double? Pressure { get; set; }
      [JsonProperty("DewPoint")] public double? DewPoint { get; set; }
      [JsonProperty("SkyTemperature")] public double? SkyTemperature { get; set; }
      [JsonProperty("CloudCoverage")] public double? CloudCoverage { get; set; }
      [JsonProperty("SkyBrightness")] public double? SkyBrightness { get; set; }
      [JsonProperty("SkyQuality")] public double? SkyQuality { get; set; }
      [JsonProperty("IsSafe")] public bool? IsSafe { get; set; }
      [JsonIgnore] public static DateTime LastUpdate => null != ms_Instance ? ms_Instance.MeasureDate.Value : DateTime.Now.AddDays(-1);
      [JsonIgnore] private static CurrentConditions ms_Instance;

      public static CurrentConditions GetInstance(string datasUrl) => GetInstance(datasUrl, false);
      public static CurrentConditions GetInstance(string datasUrl, bool forceRefresh) {
         if (null == ms_Instance || forceRefresh || Math.Round((DateTime.Now - LastUpdate).TotalMinutes, 0) > REFRESH_TIMEOUT) {
            using (WebClient wc = new WebClient()) {
               wc.Headers[HttpRequestHeader.ContentType] = "application/json";
               try {
                  var json = wc.DownloadString(datasUrl);
                  ms_Instance = JsonConvert.DeserializeObject<CurrentConditions>(json);
               } catch (Exception) {
                  var json = wc.DownloadString(datasUrl);
                  ms_Instance = JsonConvert.DeserializeObject<CurrentConditions>(json);
               }
            }
         }
         return ms_Instance;
      }
      public override string ToString() => JsonConvert.SerializeObject(this);
   }
}
