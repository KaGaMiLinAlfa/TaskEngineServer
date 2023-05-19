using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;

namespace TaskEngine
{
    public class Engine
    {

        public Engine()
        {

        }












        private static IScheduler scheduler;
        private static void CreateScheduler()
        {
            var config = new NameValueCollection() {
                { "quartz.jobStore.misfireThreshold", "800" }
            };

            scheduler = new StdSchedulerFactory(config).GetScheduler().Result;

            var job = JobBuilder.Create(typeof(TaskSchedulerManager)).WithIdentity("Engine").Build();
            var trigger = TriggerBuilder.Create().WithIdentity("Engine")
                .StartAt(DateTimeOffset.Now)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).WithMisfireHandlingInstructionIgnoreMisfires())
                .Build();
        }


    }

    [DisallowConcurrentExecution]
    public class TaskSchedulerManager : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            UpdateScheduler();

            return Task.CompletedTask;
        }



        private void GetTaskList()
        {

        }

        private void UpdateScheduler()
        {





        }

    }


    public class Worker : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {

            List<string> ss = new List<string>();

            ss.CopyTo


            //rpc
            return Task.CompletedTask;
        }




    }
}
