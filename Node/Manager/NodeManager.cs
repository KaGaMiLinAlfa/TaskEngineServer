using Node.Helper;
using Node.Model;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Node.Manager
{
    public class NodeManager
    {
        private readonly string NodeId = "11";
        IScheduler scheduler;
        private List<int> NodeTaskIds = new List<int>();
        string DLLSavePath = Path.Combine(Directory.GetCurrentDirectory(), "DownloadFile");


        private Dictionary<int, QuartzModel> Worker = new Dictionary<int, QuartzModel>();

        public NodeManager()
        {
            var config = new NameValueCollection()
            {
                { "quartz.jobStore.misfireThreshold", "800" }
            };

            scheduler = new StdSchedulerFactory(config).GetScheduler().Result;
            scheduler.Start();
        }

        public async Task RunAsync()
        {

            while (true)
            {
                try
                {
                    Console.WriteLine($"刷新心跳");
                    CheckTaskInfo();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"刷新异常:{ex.Message}");
                    await Task.Delay(1000 * 5);
                }

                await Task.Delay(1000 * 5);
            }
        }

        private void CheckTaskInfo()
        {
            //1.节点状态更新


            //2.更新Worker状态
            UpdateNodeTask();

            //3.清理过期的节点DLL文件
            //DeleteOutdatedDLLFiles();

            //4.节点状态更新

        }

        private void UpdateNodeTask()
        {
            var tasks = GetNodeTasksInfo();
            var taskIds = tasks.Select(s => s.Id).ToList();
            var needAddTaskIds = taskIds.Except(Worker.Keys).ToList();
            var needStopTaskIds = NodeTaskIds.Except(taskIds).ToList();

            if (needAddTaskIds.Count <= 0 && needStopTaskIds.Count <= 0)
                return;

            if (needStopTaskIds.Any())
                foreach (var item in needStopTaskIds)
                    if (!Worker.ContainsKey(item))
                        RemoveJob(Worker[tasks.First(x => x.Id == item).Id]);

            if (needAddTaskIds.Any())
                foreach (var item in needAddTaskIds)
                    AddJob(tasks.First(x => x.Id == item));

        }

        public void AddJob(TaskInfo newTask)
        {
            var job = JobBuilder.Create(typeof(WorkManager)).WithIdentity(newTask.TaskId, NodeId).Build();
            var trigger = TriggerBuilder.Create().WithIdentity(newTask.TaskId, NodeId)
                .WithCronSchedule(newTask.Cron, x => x.WithMisfireHandlingInstructionDoNothing()).Build();

            var taskDllPath = $"{ZipHelper.GetTaskDllPath(newTask.PackageUrl, Path.Combine(DLLSavePath, $"V{newTask.PackageId}-{newTask.Id}"))}";

            var jobModel = new QuartzModel
            {
                Job = job,
                Trigger = trigger,
                TaskInfo = newTask,
                TaskDllPath = taskDllPath
            };

            //if (string.IsNullOrEmpty(jobModel.TaskDllPath))
            //需要写上上传包没有dll文件的报错日志;

            job.JobDataMap.Put("QuartzModel", jobModel);
            Worker.Add(newTask.Id, jobModel);
            scheduler.ScheduleJob(job, trigger);
        }

        public void RemoveJob(QuartzModel quartzModel)
        {
            var task1 = scheduler.PauseJob(quartzModel.Job.Key);
            var task2 = scheduler.UnscheduleJob(quartzModel.Trigger.Key);
            var task3 = scheduler.DeleteJob(quartzModel.Job.Key);

            scheduler.Interrupt(quartzModel.Job.Key);
            Task.WaitAll(task1, task2, task3);

            Worker.Remove(quartzModel.TaskInfo.Id);
        }

        private void DeleteOutdatedDLLFiles()
        {
            var allPackageIds = GetAllTasksPackageId();
            var allFileName = Directory.GetDirectories(DLLSavePath);

            var useingFileName = allFileName.Select(s => Regex.Match(s, @"^V\d+").Value).Where(x => !string.IsNullOrEmpty(x)).ToList();

            var needDeleteFileNames = allFileName.Except(useingFileName);
            foreach (var path in needDeleteFileNames)
                Directory.Delete(path, true);
        }

        private List<TaskInfo> GetNodeTasksInfo()
        {
            return DB.FSql.Select<TaskInfo>().Where(x => x.Id > 0).ToList();
            //return new List<TaskInfo>();
        }

        private List<int> GetAllTasksPackageId()
        {
            return DB.FSql.Select<TaskInfo>().Where(x => x.Id > 0).ToList(s => s.PackageId);
            //return new List<int>();
        }
    }

    public class QuartzModel
    {
        public IJobDetail Job { get; set; }
        public ITrigger Trigger { get; set; }

        public bool IsCancel { get; set; }
        public bool IsCompleted { get; set; }

        public string TaskDllPath { get; set; }
        public string TaskClass { get; set; }
        public string Config { get; set; }

        public TaskInfo TaskInfo { get; set; }
        public bool IsRun { get; set; }
    }

}
