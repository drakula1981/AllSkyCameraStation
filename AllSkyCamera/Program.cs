using AllSkyCameraConditionService.Jobs;
using AllSkyCameraConditionService.Model;
using Microsoft.Extensions.Hosting;
using Quartz;
using Serilog;
using System.Configuration;

namespace AllSkyCameraConditionService {
   public class Program {
      public static void Main(string[] args) {
         Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
         CreateHostBuilder(args).Build().Run();
      }

      public static IHostBuilder CreateHostBuilder(string[] args) =>
         Host.CreateDefaultBuilder(args)
             .UseSerilog()
             .ConfigureServices((hostContext, services) => {
                services.AddQuartz(q => {
                   q.ScheduleJob<Conditions>(meteoTrigger => meteoTrigger.WithIdentity("ReadWeatherDatasTrigger", "weatherTriggers")
                     .StartNow()
                     .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(AppParams.MeasureInterval)
                        .RepeatForever())
                     , meteoJob => meteoJob.WithIdentity("ReadWeatherDatas", "weather"));
                });
                services.AddQuartz(q => {
                   q.ScheduleJob<CloudSensor>(CloudWatcherTrigger => CloudWatcherTrigger.WithIdentity("CloudWatcherTrigger", "weatherTriggers")
                     .StartAt(DateTimeOffset.Now.AddSeconds(30))
                     .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(AppParams.MeasureInterval)
                        .RepeatForever())
                     , meteoJob => meteoJob.WithIdentity("CloudWatcherDatas", "weather"));
                });
                services.AddQuartz(q => {
                   q.ScheduleJob<HistoryLogger>(HistoryLoggerTrigger => HistoryLoggerTrigger.WithIdentity("HistoryLoggerTrigger", "loggerTriggers")
                     .StartNow()
                     .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(AppParams.MeasureInterval - 1)
                        .RepeatForever())
                     , LogJob => LogJob.WithIdentity("LogHistory", "Log"));
                });
                services.AddQuartzHostedService(options => {
                   options.WaitForJobsToComplete = true;
                });
                services.AddQuartzServer(options => {
                   options.WaitForJobsToComplete = true;
                });
             });
   }
}