using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CarppiWebService.DeliveryJobSchedule;

namespace CarppiWebService.DeliveryJobSchedule
{
    public class TimeoutJobScheduer
    {
        public static void Start()
        {
            // ISchedulerFactory sceduler_factoru = new StdSchedulerFactory();
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<DeliverManSesionTimeout>().Build();
            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(60)
            .RepeatForever())
            .Build();




            IJobDetail job2 = JobBuilder.Create<CarppiWebService.DeliveryJobSchedule.RestaurantClosingSchedule>().Build();
            ITrigger trigger2 = TriggerBuilder.Create()
    .WithIdentity("trigger2", "group2")
    .StartNow()
    .WithSimpleSchedule(x => x
        .WithIntervalInSeconds(30)
        .RepeatForever())
    .Build();



            IJobDetail job3 = JobBuilder.Create<CarppiWebService.DeliveryJobSchedule.OrderTimeot>().Build();
            ITrigger trigger3 = TriggerBuilder.Create()
    .WithIdentity("trigger3", "group3")
    .StartNow()
    .WithSimpleSchedule(x => x
        .WithIntervalInSeconds(30)
        .RepeatForever())
    .Build();



            IJobDetail job4 = JobBuilder.Create<CarppiWebService.DeliveryJobSchedule.PeriodcPushReminder>().Build();
            ITrigger trigger4 = TriggerBuilder.Create()
    .WithIdentity("trigger4", "group4")
    .StartNow()
    .WithSimpleSchedule(x => x
        .WithIntervalInSeconds(60)
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
            scheduler.ScheduleJob(job3, trigger3);
            scheduler.ScheduleJob(job4, trigger4);
        }
    }
}