using AllSkyCameraConditionService.Jobs;
using AllSkyCameraConditionService.Model;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.AspNetCore;
using Serilog;

namespace AllSkyCameraConditionService;
public class Program {

   public static void Main(string[] args) {
      Log.Logger = new LoggerConfiguration()
         .Enrich.FromLogContext()
         .WriteTo.Console()
         .WriteTo.File(AppParams.LogFilePath, Serilog.Events.LogEventLevel.Error)
         .CreateLogger();
      CreateHostBuilder(args).Build().Run();
   }

   public static IHostBuilder CreateHostBuilder(string[] args) {
      var sunDatas = EphemeridClient.GetSunDatas().Result;
      //Log.Information($"{sunDatas}");
      return Host.CreateDefaultBuilder(args)
          .UseSerilog()
          .ConfigureServices((hostContext, services) => {
             services.AddQuartz(q => {
                q.ScheduleJob<Conditions>(meteoTrigger => meteoTrigger.WithIdentity("ReadWeatherDatasTrigger", "conditionsTriggers")
                  .StartNow()
                  .WithSimpleSchedule(x => x
                     .WithIntervalInMinutes(AppParams.MeasureInterval)
                     .RepeatForever())
                  , meteoJob => meteoJob.WithIdentity("ReadWeatherDatas", "conditions"));
             });
             services.AddQuartz(q => {
                q.ScheduleJob<CloudSensor>(CloudWatcherTrigger => CloudWatcherTrigger.WithIdentity("CloudWatcherTrigger", "conditionsTriggers")
                 .WithCronSchedule($"0 0/{AppParams.MeasureInterval} {sunDatas.Setting.Hour[..2]}-{sunDatas.Rising.Hour[..2]} * * ?")
                 , meteoJob => meteoJob.WithIdentity("CloudWatcherDatas", "conditions"));
             });
             services.AddQuartz(q => {
                q.ScheduleJob<SkyLuminance>(SkyQualityTrigger => SkyQualityTrigger.WithIdentity("SkyQualityTrigger", "conditionsTriggers")
                 .WithCronSchedule($"0 0/{AppParams.MeasureInterval + 2} {sunDatas.Setting.Hour[..2]}-{sunDatas.Rising.Hour[..2]} * * ?")
                  , meteoJob => meteoJob.WithIdentity("SkyQualityDatas", "conditions"));
             });
             services.AddQuartz(q => {
                q.ScheduleJob<HistoryLogger>(HistoryLoggerTrigger => HistoryLoggerTrigger.WithIdentity("HistoryLoggerTrigger", "loggerTriggers")
                  .StartAt(DateTimeOffset.Now.AddSeconds(60))
                  .WithSimpleSchedule(x => x
                     .WithIntervalInMinutes(0 == AppParams.MeasureInterval - 1 ? 1 : AppParams.MeasureInterval - 1)
                     .RepeatForever())
                  , LogJob => LogJob.WithIdentity("LogHistory", "Log")
                                    .UsingJobData("SunRiseSetDatas", sunDatas.ToString()));
             });
             services.AddQuartz(q => {
                q.ScheduleJob<ServiceWebServer>(ServiceWebServerTrigger => ServiceWebServerTrigger.WithIdentity("ServiceWebServer", "webTriggers")
                  .StartNow()
                  , WebJob => WebJob.WithIdentity("ServiceWebServer", "Web"));
             });
             services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = false; });
             services.AddQuartzServer(options => { options.WaitForJobsToComplete = false; });
          });
   }
}