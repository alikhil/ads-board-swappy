using NLog;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Swappy_V2.Classes
{
    public class JobScheduler
    {
        public static void Start()
        {

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<ClearJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(3, 45))
                  )
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
    public class ClearJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Info("Начато удаление временных изображений");
            var dirName = AppConstants.TempImageFullPath;
            var all = Directory.EnumerateFiles(dirName);
            long cnt = 0;
            foreach (var fname in all)
            {
                var file = new FileInfo(fname);
                file.Delete();
                cnt++;
            }
            logger.Info("Удаление временных изобржений завершено. Удаленно " + cnt + " файлов");

        }
    }
}