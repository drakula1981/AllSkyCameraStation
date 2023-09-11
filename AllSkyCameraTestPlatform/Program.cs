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
      Log.Information($"Now : {Moon.UtcNow()}");
      Log.Information($"Tomorrow : {Moon.Calculate(DateTime.UtcNow.AddDays(1))}");
      Log.Information($"Next week : {Moon.Calculate(DateTime.UtcNow.AddDays(7))}");
      Log.Information($"Next 2 weeks : {Moon.Calculate(DateTime.UtcNow.AddDays(14.6))}");
      Log.Information($"Next 2 weeks : {Moon.Calculate(DateTime.UtcNow.AddDays(18))}");
      Log.Information($"Next Month : {Moon.Calculate(DateTime.UtcNow.AddDays(30))}");
      Console.ReadLine();
   }
}
