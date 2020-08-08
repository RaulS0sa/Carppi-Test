using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarppiWebService.ClaseDescerializadora
{ 
    public class JobScheduler
    {
        public static void Start()
        {
           // ISchedulerFactory sceduler_factoru = new StdSchedulerFactory();
            IScheduler scheduler =StdSchedulerFactory.GetDefaultScheduler().Result;
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<ScheduledTasks>().Build();
            ITrigger trigger = TriggerBuilder.Create()
    .WithIdentity("trigger1", "group1")
    .StartNow()
    .WithSimpleSchedule(x => x
        .WithIntervalInSeconds(1800)
        .RepeatForever())
    .Build();




            IJobDetail job2 = JobBuilder.Create<ScheduledTasksForCoins>().Build();
            ITrigger trigger2 = TriggerBuilder.Create()
    .WithIdentity("trigger2", "group2")
    .StartNow()
    .WithSimpleSchedule(x => x
        .WithIntervalInSeconds(1800)
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
            scheduler.ScheduleJob(job2, trigger2);
        }
    }
}