using AllSkyCameraTestPlatform.Model;
using Serilog;

public class Program {

   public static async Task Main(string[] args) {
      Log.Logger = new LoggerConfiguration()
         .Enrich.FromLogContext()
         .WriteTo.Console()
         .CreateLogger();
      //var sun = await EphemeridClient.GetSunDatas();
      //Log.Information($"{sun}");
      var moon = Moon.UtcNow();
      Log.Information($"{moon}");
      Console.ReadLine();
   }
}
