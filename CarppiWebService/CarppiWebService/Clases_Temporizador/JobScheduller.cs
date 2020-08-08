using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarppiWebService.Clases_Temporizador
{
    public class JobScheduler
    {
        public static void Start()
        {
            // ISchedulerFactory sceduler_factoru = new StdSchedulerFactory();
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<PeriodicTask_Carppi>().Build();
            ITrigger trigger = TriggerBuilder.Create()
    .WithIdentity("trigger2", "group1")
    .StartNow()
    .WithSimpleSchedule(x => x
        .WithIntervalInSeconds(10)
        .RepeatForever())
    .Build();
            /*
            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                  )
                .Build();
            */

            scheduler.ScheduleJob(job, trigger);
        }
    }
}