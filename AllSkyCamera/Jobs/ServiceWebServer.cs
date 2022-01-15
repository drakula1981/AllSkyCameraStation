using AllSkyCameraConditionService.Model;
using EmbedIO;
using EmbedIO.Actions;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using Swan.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllSkyCameraConditionService.Jobs {
   internal class ServiceWebServer : IJob, IDisposable {
      private WebServer? Server { get; set; }
      public async Task Execute(IJobExecutionContext context) {
         Server = CreateWebServer(AppParams.ApiAccessUrl);
         // Once we've registered our modules and configured them, we call the RunAsync() method.
         await Server.RunAsync();
      }

      private static WebServer CreateWebServer(string url) {
         var server = new WebServer(o => o
                 .WithUrlPrefix(url)
                 .WithMode(HttpListenerMode.EmbedIO))
             .WithLocalSessionManager()
             .WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { CurrentConditions = DataHisto.Instance.GetCurrentConditions() })));

         // Listen for state changes.
         server.StateChanged += (s, e) => Log.Logger.Information($"WebServer New State - {e.NewState}");
         return server;
      }

      public void Dispose() {
         Dispose(true);
      }

      public void Dispose(bool disposing) {
         if (!disposing) return;
         Server?.Dispose();
      }
   }
}
