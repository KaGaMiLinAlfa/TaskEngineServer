using BaseTaskManager;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using Ionic.Zip;
using System.Reflection;
using System;
using System.Threading.Tasks;

namespace Worker
{
    public static class WorkerServices
    {
        private static BaseTask task;
        private static bool IsRunning = false;
        private static object LockObj = new object();
        private static short TaskStats;

        public static void Build()
        {
            if (task != null)
                return;

            var classname = "ClassLibrary3.TestTask";
            var dllpackurl = "";
            var packageId = "";
            var taskid = "";
            var dLLSavePath = Path.Combine(Directory.GetCurrentDirectory(), "DownloadFile");
            //var taskDllPath = $"{ZipHelper.GetTaskDllPath(dllpackurl, Path.Combine(dLLSavePath, $"V{packageId}"))}-{taskid}";
            var taskDllPath = @"E:\Users\KaGaMi\source\repos\YLFERPSync\ClassLibrary3\bin\Debug\netcoreapp3.1\ClassLibrary3.dll";


            var assembly = Assembly.LoadFrom(taskDllPath);// dll路径
            var type = assembly.GetType(classname); // 获取该dll中命名空间类

            var ttts = assembly.GetTypes();
            var obj = Activator.CreateInstance(type, new object[] { });// 实例化该类
            if (!(obj is BaseTask))
                throw new Exception("实例化类型基类并非BaseTask类型!");

            task = obj as BaseTask;
        }

        public static void Run()
        {
            if (task == null)
                return;

            lock (LockObj)
            {
                if (IsRunning)
                    return;

                IsRunning = true;
            }
            try
            {
                task.Run();
            }
            catch (Exception ex)
            {


            }

            DisposeTask();
            IsRunning = false;
        }

        public static async void DisposeTask()
        {
            if (task == null)
                return;

            while (true)
            {
                if (!IsRunning)
                {
                    task = null;
                    break;
                }

                await Task.Delay(3000);
            }
        }

    }

    public class ZipHelper
    {
        public static void Extract(string zipPathName, string extractPath)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (ZipFile zip = new ZipFile(zipPathName, Encoding.GetEncoding("GB2312")))
                zip.ExtractAll(extractPath, ExtractExistingFileAction.OverwriteSilently);
        }

        #region Download Task Dll Package

        public static string GetTaskDllPath(string url, string savePath)
        {
            //var savePath = Path.Combine(Directory.GetCurrentDirectory(), "DownloadFile", $"V{_v}");

            if (Directory.Exists(savePath) && Directory.GetFiles(savePath, "*.dll").Any())
                return savePath;

            var savePathName = DownloadFile(url, savePath);

            ZipHelper.Extract(savePathName, savePath);
            var copyDllPath = GetDirDllPath(savePath);

            if (copyDllPath != savePath)
                CopyDirAllFile(copyDllPath, savePath);

            return savePath;
        }

        public static string DownloadFile(string url, string savePath)
        {
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            using var client = new WebClient();
            var savePathName = Path.Combine(savePath, Path.GetFileName(url));
            client.DownloadFile(url, savePathName);

            return savePathName;
        }

        /// <summary>
        /// 深度优先历遍dll
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetDirDllPath(string path)
        {
            if (Directory.GetFiles(path, "*.dll").Any())
                return path;

            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                if (Directory.GetFiles(dir, "*.dll").Any())
                    return dir;

                var chiDir = GetDirDllPath(dir);
                if (!string.IsNullOrEmpty(chiDir))
                    return chiDir;
            }
            return null;
        }

        private static string GetDirDllPath2(string path)
        {
            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                if (Directory.GetFiles(dir, "*.dll").Any())
                    return dir;

                var chiDir = GetDirDllPath2(dir);
                if (!string.IsNullOrEmpty(chiDir))
                    return chiDir;
            }
            return null;
        }


        private static void CopyDirAllFile(string oldPath, string newPath) => CopyDirAllFile(new DirectoryInfo(oldPath), newPath);
        private static void CopyDirAllFile(DirectoryInfo oldDir, string newPath)
        {

            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);

            var files = oldDir.GetFiles();
            foreach (var file in files)
                file.CopyTo(Path.Combine(newPath, file.Name), true);

            var dirs = oldDir.GetDirectories();
            foreach (var dis in dirs)
                CopyDirAllFile(dis, Path.Combine(newPath, dis.Name));
        }
        #endregion

    }

}
