using BaseTaskManager;
using Node.Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Node.Manager
{
    [DisallowConcurrentExecution]
    public class WorkManager : IJob
    {

        private QuartzModel quartzModel;

        public async Task Execute(IJobExecutionContext context)
        {
            quartzModel = context.JobDetail.JobDataMap.Get("QuartzModel") as QuartzModel;
            await Console.Out.WriteLineAsync($"Pahe:{quartzModel.TaskDllPath}->{DateTime.Now}:任务开始 -- Name = {quartzModel.TaskInfo.Name}");





        }
    }

    public class AssemblyInstantiation : IExecutePackageAction
    {
        string _dllPath, _className;

        public AssemblyInstantiation()
        {

        }


        public void Execute(TaskInfo taskInfoMode)
        {
            var assembly = Assembly.LoadFrom(_dllPath);// dll路径
            var type = assembly.GetType(taskInfoMode.ClassPath); // 获取该dll中命名空间类
            object obj = Activator.CreateInstance(type, new object[] { taskInfoMode });// 实例化该类
            if (!(obj is BaseTask))
                throw new Exception("错误类型!");

            (obj as BaseTask).Run();
        }





        public void SetPath(string dllPath, string className, string config)
        {
            _dllPath = dllPath;
            _className = className;
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }
    }

    public class ChiProcess : IExecutePackageAction
    {
        Process TaskProcess;
        string _arguments;
        string _nodeTaskPath;

        public void Execute(TaskInfo taskInfoMode)
        {
            TaskProcess = new Process();
            TaskProcess.StartInfo.FileName = @"dotnet";
            TaskProcess.StartInfo.Arguments = _arguments;
            TaskProcess.Start();

            TaskProcess.WaitForExit();
        }

        public void Cancel()
        {
            TaskProcess.Kill();
        }

        public void SetPath(string dllPath, string className, string config)
        {
            _nodeTaskPath = "ConsoleWorkerUrl";
            _arguments = $"{_nodeTaskPath} \"{dllPath}\" \"{className}\" \"{config}\"";
        }
    }


    public interface IExecutePackageAction
    {
        public void Execute(TaskInfo taskInfoMode);

        public void Cancel();

        public void SetPath(string dllPath, string className, string config);
    }
}
