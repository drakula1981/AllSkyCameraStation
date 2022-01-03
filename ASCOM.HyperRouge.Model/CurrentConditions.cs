using Newtonsoft.Json;
using System;
using System.Net;

namespace ASCOM.HyperRouge.Model {
   [JsonObject("CurrentConditions")]
   public class CurrentConditions {
      private const double REFRESH_TIMEOUT = 3;
      [JsonProperty("lastWeatherDatas")] private WeatherDatas LastWeatherDatas { get; set; }
      [JsonProperty("lastCloudWatch")] private CloudTemperatureDatas LastCloudWatch { get; set; }

      [JsonIgnore] private static CurrentConditions ms_Instance;
      [JsonIgnore] public double Temperature => ms_Instance.LastWeatherDatas.Temperature;
      [JsonIgnore] public double Humidity => ms_Instance.LastWeatherDatas.Humidity;
      [JsonIgnore] public double DewPoint => ms_Instance.LastWeatherDatas.DewPoint;
      [JsonIgnore] public double Pressure => ms_Instance.LastWeatherDatas.Pressure;
      [JsonIgnore] public double CloudCoverage => ms_Instance.LastCloudWatch.CloudCoveragePercent;
      [JsonIgnore] public double SkyTemperature => ms_Instance.LastCloudWatch.SkyTemperature;
      [JsonIgnore] public bool IsSafe => ms_Instance.LastCloudWatch.IsSafe;
      [JsonIgnore] public static DateTime LastUpdate => ms_Instance.LastWeatherDatas.MeasureDate;

       public static CurrentConditions GetInstance(string datasUrl) {
            if (null == ms_Instance || Math.Round((DateTime.Now - LastUpdate).TotalMinutes,0) > REFRESH_TIMEOUT) {
               using (WebClient wc = new WebClient()) {
                  wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                  var json = wc.DownloadString(datasUrl);
                  ms_Instance = JsonConvert.DeserializeObject<CurrentConditions>(json);
               }
            }
            return ms_Instance;
      }
      public override string ToString() => JsonConvert.SerializeObject(this);
   }
}
