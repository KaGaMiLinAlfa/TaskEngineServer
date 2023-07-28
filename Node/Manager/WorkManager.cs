using BaseTaskManager;
using Node.Helper;
using Node.Model;
using Quartz;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Node.Manager
{
    [DisallowConcurrentExecution]
    public class WorkManager : IJob
    {

        private QuartzModel quartzModel;
        private bool _isInit;

        public async Task Execute(IJobExecutionContext context)
        {
            quartzModel = context.JobDetail.JobDataMap.Get("QuartzModel") as QuartzModel;

            //如果需要停止的话,则下次不能再执行了
            if (quartzModel.IsStop || quartzModel.WaitingToStop || quartzModel.NeedAbort)
                return;

            //if (quartzModel.timer == null)
            //{
            //    quartzModel.timer = new System.Threading.Timer(TaskHeartbeatTime, new AutoResetEvent(false), 1000, 5000);
            //    await Console.Out.WriteLineAsync($"第一周期启动心跳并立即结束");
            //    return;
            //}

            await Console.Out.WriteLineAsync($"Pahe:{quartzModel.TaskDllPath}->{DateTime.Now}:任务开始 -- Name = {quartzModel.TaskInfo.TaskName}");

            if (quartzModel.Executer == null || !quartzModel.Executer.IsRunning)
                quartzModel.Executer = new ChiProcess();

            var dllfilepath = Path.Combine(quartzModel.TaskDllPath, quartzModel.TaskInfo.DllName);
            quartzModel.Executer.SetPath(quartzModel.TaskInfo.Id.ToString(), dllfilepath, quartzModel.TaskInfo.ClassPath, quartzModel.Config);
            quartzModel.Executer.Execute(quartzModel.TaskInfo);

            Console.WriteLine("BaseTask结束锚点");
        }
    }

    public class ChiProcess : IExecutePackageAction
    {
        Process TaskProcess;
        string _arguments;
        string _nodeTaskPath;

        public bool IsRunning { get; set; }

        public void Execute(TaskInfo taskInfoMode)
        {
            if (taskInfoMode.Stats != 2)
            {
                Console.WriteLine("状态检查异常停止当前周期执行");
                return;
            }

            IsRunning = true;

            taskInfoMode.Stats = 3;
            var update = DB.FSql.Update<TaskInfo>()
                  .Set(x => x.LastHeartbeatTime == DateTime.Now)
                  .Set(x => x.Stats == 3)
                  .Where(x => x.Id == taskInfoMode.Id && x.Stats == 2)
                  .ExecuteAffrows();

            if (update <= 0)
            {
                Console.WriteLine("更新状态失败");
                return;
            }


            Console.WriteLine($"_arguments:{_arguments}");
            TaskProcess = new Process();
            TaskProcess.StartInfo.FileName = @"dotnet";
            TaskProcess.StartInfo.Arguments = _arguments;
            TaskProcess.Start();

            TaskProcess.WaitForExit();


            taskInfoMode.Stats = 2;
            DB.FSql.Update<TaskInfo>()
                .Set(x => x.LastHeartbeatTime == DateTime.Now)
                .Set(x => x.Stats == 2)
                .Where(x => x.Id == taskInfoMode.Id && x.Stats == 3)
                .ExecuteAffrows();

            IsRunning = false;
        }

        public void Cancel()
        {
            TaskProcess.Kill();
            IsRunning = false;
        }

        public void SetPath(string taskid, string dllPath, string className, string config)
        {
            _nodeTaskPath = Assembly.GetExecutingAssembly().Location;
            //_arguments = @$"{_nodeTaskPath} Worker ""{dllPath}"" ""{className}"" ""{config}""";
            _arguments = @$"{_nodeTaskPath} Worker ""{taskid}"" ""{dllPath}"" ""{className}""";
        }

        public static void RunTask(string taskid, string dllPath, string className, string config)
        {
            Assembly.Load(File.ReadAllBytes(dllPath));
            var assembly = Assembly.LoadFrom(dllPath);// dll路径
            var type = assembly.GetType(className); // 获取该dll中命名空间类
            object obj = Activator.CreateInstance(type);// 实例化该类
            if (!(obj is BaseTask))
                throw new Exception("错误类型!");


            #region MyRegion
            //var fullPath = Path.GetFullPath(dllPath);
            //var clx = new CustomLoadContext(fullPath); // initialize custom context
            //var assembly = clx.LoadFromStream(new MemoryStream(File.ReadAllBytes(fullPath))); // load your desired assembly
            //var type = assembly.GetType(className); // 获取该dll中命名空间类
            //object obj = Activator.CreateInstance(type);// 实例化该类


            //MethodInfo method = type.GetMethod("Run");//Loading
            //MethodInfo method2 = type.GetMethod("Init");//Loading

            //method2.Invoke(obj, null);
            ////传入所调方法需要的参数
            //method.Invoke(obj, null);//Execute
            #endregion


            var baseTask = (obj as BaseTask);

            baseTask.TaskId = int.Parse(taskid);

            baseTask.Init();
            baseTask.Run();

            Console.WriteLine("Worker结束锚点");
        }
    }

    public class CustomLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver resolver;

        public CustomLoadContext(string mainAssemblyToLoadPath) : base(isCollectible: true)
        {
            resolver = new AssemblyDependencyResolver(mainAssemblyToLoadPath);
        }


        protected override Assembly Load(AssemblyName name)
        {
            Console.WriteLine("Resolving : {0}", name.FullName);
            var assemblyPath = resolver.ResolveAssemblyToPath(name);
            if (assemblyPath != null)
            {
                return Assembly.Load(File.ReadAllBytes(assemblyPath));
            }

            return Assembly.Load(name);
        }
    }


    public interface IExecutePackageAction
    {
        public void Execute(TaskInfo taskInfoMode);

        public void Cancel();

        public void SetPath(string taskid, string dllPath, string className, string config);

        public bool IsRunning { get; set; }
    }
}
