using AllSkyCameraConditions.Interfaces;
using AllSkyCameraConditions.Interfaces.AstronomicDatas;
using AllSkyCameraConditionService.Model.AstronomicDatas;
using Newtonsoft.Json;
using Serilog;
using System.Web;

namespace AllSkyCameraConditions.Services.AstronomicDatas;
public class EphemeridMeasureService : IEphemeridMeasureService {
   private const string IMCCE_API_URL = "https://ssp.imcce.fr/webservices/miriade/api/rts.php?-body=11&-nbd=1&-step=1&-observer=###COORDS###&-ep=now&-twilight=1&-tz=2&-mime=json";

   public async Task<Root?> ReadMeasure(double? measureContext1 = null, double? measureContext2 = null, double? measureContext3 = null, double? measureContext4 = null, double? measureContext5 = null) {
      try {
         using HttpClient client = new();
         var apiUrl = IMCCE_API_URL.Replace("###COORDS###", HttpUtility.HtmlEncode($"{measureContext1},{measureContext2},{measureContext3:n0}"));
         //Log.Logger.Information($"Calling URL:{apiUrl}");
         var response = await client.PostAsync(apiUrl, null);
         if (response != null) {
            var jsonString = await response.Content.ReadAsStringAsync();
            jsonString = jsonString[7..];
            return JsonConvert.DeserializeObject<Root>(jsonString.TrimEnd('}').TrimEnd(']'));
         }
      } catch (Exception ex) {
         Log.Logger.Error($"An error while retrieving Sun's Rise and Set datas occured: {ex.Message}");
      }
      return null;
   }

   public Root? ReadMeasure(int? measureContext1 = null, double? measureContext2 = null, bool debugMode = false) => throw new NotImplementedException();

   Root IMeasureServiceEntity<Root>.ReadMeasure(string? measureContext) => throw new NotImplementedException();
}