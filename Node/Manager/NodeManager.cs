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

            //装载本节点"待启动"的Worker
            var pendingStartTask = tasks.Where(x => x.Stats == 6);
            foreach (var item in pendingStartTask)
                if (!Worker.Keys.Contains(item.Id))
                    AddJob(item);

            //通知Worker停止或中止任务
            var stopTask = tasks.Where(x => new int[] { 4, 5 }.Contains(x.Stats) && Worker.Keys.Contains(x.Id));
            foreach (var item in stopTask)
                switch (item.Stats)
                {
                    case 4: Worker[item.Id].WaitingToStop = true; UnInstallWorker(item); break;
                    case 5: Worker[item.Id].NeedAbort = true; UnInstallWorker(item); break;
                }

            //卸载Worker已经停止的任务
            foreach (var item in Worker)
                if (item.Value.IsStop)
                    RemoveJob(Worker[item.Value.TaskInfo.Id]);

            //卸载状态为已停止容器(道理来说,不会存在这样的task,页面操作需要等待子程序结束后,再轮询卸载worker,除非改数据库大面积停止)
            var unInstallTask = tasks.Where(x => x.Stats == 1 && Worker.Keys.Contains(x.Id));
            foreach (var item in unInstallTask)
                Worker[item.Id].WaitingToStop = true;

            //停止不是本节点的task(道理来说,节点不会包含其他节点任务-非停止状态的task不能修改节点,节点自会启动本节点"待启动"的任务)
            var otherNodeTask = Worker.Keys.Except(tasks.Select(s => s.Id)).ToList();
            foreach (var item in otherNodeTask)
                Worker[item].WaitingToStop = true;


            foreach (var item in Worker)
                Console.WriteLine($"任务ID: {item.Key} 为 {item.Value.TaskInfo.Stats} 状态");

        }

        public void UnInstallWorker(TaskInfo taskInfo)
        {
            switch (taskInfo.Stats)
            {
                case 4: Worker[taskInfo.Id].WaitingToStop = true; break;
                case 5: Worker[taskInfo.Id].NeedAbort = true; break;
            }

            if (Worker[taskInfo.Id].TaskInfo.Stats == 2)
            {
                RemoveJob(Worker[taskInfo.Id]);
            }



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

            Console.WriteLine($"装载任务:{newTask.Id}");

            newTask.Stats = 2;
            DB.FSql.Update<TaskInfo>().Set(x => new TaskInfo
            {
                Stats = 2,
            }).Where(x => x.Id == newTask.Id && x.Stats == 6).ExecuteAffrows();
        }

        public void RemoveJob(QuartzModel quartzModel)
        {
            var task1 = scheduler.PauseJob(quartzModel.Job.Key);
            var task2 = scheduler.UnscheduleJob(quartzModel.Trigger.Key);
            var task3 = scheduler.DeleteJob(quartzModel.Job.Key);

            scheduler.Interrupt(quartzModel.Job.Key);
            Task.WaitAll(task1, task2, task3);

            if (quartzModel.timer != null)
                quartzModel.timer.Dispose();

            Worker.Remove(quartzModel.TaskInfo.Id);

            Console.WriteLine($"卸载任务:{quartzModel.TaskInfo.Id}");

            UpdateTaskStats(quartzModel.TaskInfo.Id, 1);
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
            return DB.FSql.Select<TaskInfo>().Where(x => x.Id == 1).ToList();
            //return new List<TaskInfo>();
        }

        private List<int> GetAllTasksPackageId()
        {
            return DB.FSql.Select<TaskInfo>().Where(x => x.Id > 0).ToList(s => s.PackageId);
            //return new List<int>();
        }
        private void UpdateTaskStats(int id, int stats)
        {
            DB.FSql.Update<TaskInfo>().Set(x => new TaskInfo
            {
                Stats = stats,
            }).Where(x => x.Id == id).ExecuteAffrows();
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


        public bool WaitingToStop { get; set; }
        public bool NeedAbort { get; set; }

        public bool IsStop { get; set; }

        public System.Threading.Timer timer { get; set; }
    }

}
