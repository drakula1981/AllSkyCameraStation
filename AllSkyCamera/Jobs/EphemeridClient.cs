using AllSkyCameraConditionService.Model;
using Newtonsoft.Json;
using Serilog;
using System.Text;
using System.Web;

namespace AllSkyCameraConditionService.Jobs {
   public class EphemeridClient {
      private const string IMCCE_API_URL = "https://ssp.imcce.fr/webservices/miriade/api/rts.php?-body=11&-nbd=1&-step=1&-observer=###COORDS###&-ep=now&-twilight=1&-tz=2&-mime=json";
      public static async Task<Root?> GetSunDatas() {
         try {
            using HttpClient client = new();
            var apiUrl = IMCCE_API_URL.Replace("###COORDS###",AppParams.SiteCoords);
            //Log.Logger.Information($"Calling URL:{apiUrl}");
            var response = await client.PostAsync(apiUrl, null);
            if (response != null) {
               var jsonString = await response.Content.ReadAsStringAsync();
               jsonString = jsonString[7..];
               //Log.Logger.Information($"API response raw:{jsonString.TrimEnd('}').TrimEnd(']')}");
               return JsonConvert.DeserializeObject<Root>(jsonString.TrimEnd('}').TrimEnd(']'));
            }
         } catch (Exception ex) {
            Log.Logger.Error($"An error while retrieving Sun's Rise and Set datas occured: {ex.Message}");
         }
         return null;
      }
   }
}
