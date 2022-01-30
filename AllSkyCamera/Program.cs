﻿using AllSkyCameraConditionService.Jobs;
using AllSkyCameraConditionService.Model;
using Microsoft.Extensions.Hosting;
using Quartz;
using Serilog;

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
                   q.ScheduleJob<Conditions>(meteoTrigger => meteoTrigger.WithIdentity("ReadWeatherDatasTrigger", "conditionsTriggers")
                     .StartNow()
                     .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(AppParams.MeasureInterval)
                        .RepeatForever())
                     , meteoJob => meteoJob.WithIdentity("ReadWeatherDatas", "conditions"));
                });
                services.AddQuartz(q => {
                   q.ScheduleJob<CloudSensor>(CloudWatcherTrigger => CloudWatcherTrigger.WithIdentity("CloudWatcherTrigger", "conditionsTriggers")
                     .StartAt(DateTimeOffset.Now.AddSeconds(30))
                     .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(AppParams.MeasureInterval)
                        .RepeatForever())
                     , meteoJob => meteoJob.WithIdentity("CloudWatcherDatas", "conditions"));
                });
                services.AddQuartz(q => {
                   q.ScheduleJob<SkyLuminance>(SkyQualityTrigger => SkyQualityTrigger.WithIdentity("SkyQualityTrigger", "conditionsTriggers")
                     .StartAt(DateTimeOffset.Now.AddSeconds(30))
                     .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(AppParams.MeasureInterval)
                        .RepeatForever())
                     , meteoJob => meteoJob.WithIdentity("SkyQualityDatas", "conditions"));
                });
                services.AddQuartz(q => {
                   q.ScheduleJob<HistoryLogger>(HistoryLoggerTrigger => HistoryLoggerTrigger.WithIdentity("HistoryLoggerTrigger", "loggerTriggers")
                     .StartNow()
                     .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(AppParams.MeasureInterval - 1)
                        .RepeatForever())
                     , LogJob => LogJob.WithIdentity("LogHistory", "Log"));
                });
                services.AddQuartz(q => {
                   q.ScheduleJob<ServiceWebServer>(ServiceWebServerTrigger => ServiceWebServerTrigger.WithIdentity("ServiceWebServer", "webTriggers")
                     .StartNow()
                     , WebJob => WebJob.WithIdentity("ServiceWebServer", "Web"));
                });
                services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = false; });
                services.AddQuartzServer(options => { options.WaitForJobsToComplete = true; });
             });
   }
}