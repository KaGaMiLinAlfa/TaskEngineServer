
using Ionic.Zip;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace TaskEngineServer.Helper
{
    public class ZipHelper
    {
        public static void Extract(string zipPathName, string extractPath)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (ZipFile zip = new ZipFile(zipPathName, Encoding.GetEncoding("GB2312")))
                zip.ExtractAll(extractPath, ExtractExistingFileAction.OverwriteSilently);
        }

        #region Download Task Dll Package

        public string GetTaskDllPath(string url, int _v)
        {
            var savePath = Directory.GetCurrentDirectory() + $"\\DownloadFile\\V{_v}\\";

            if (Directory.Exists(savePath) && Directory.GetFiles(savePath, "*.dll").Any())
                return savePath;

            var savePathName = DownloadFile(url, savePath);

            ZipHelper.Extract(savePathName, savePath);
            var copyDllPath = GetDirDllPath(savePath);

            if (copyDllPath != savePath)
                CopyDirAllFile(copyDllPath, savePath);

            return savePath;
        }

        public string DownloadFile(string url, string savePath)
        {
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            WebClient client = new WebClient();
            var savePathName = savePath + Path.GetFileName(url);
            client.DownloadFile(url, savePathName);

            return savePathName;
        }

        /// <summary>
        /// 深度优先历遍dll
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetDirDllPath(string path)
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

        public void CopyDirAllFile(string oldPath, string newPath) => CopyDirAllFile(new DirectoryInfo(oldPath), newPath);
        public void CopyDirAllFile(DirectoryInfo oldDir, string newPath)
        {

            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);

            var files = oldDir.GetFiles();
            foreach (var file in files)
                file.CopyTo(newPath + $"\\{file.Name}", true);

            var dirs = oldDir.GetDirectories();
            foreach (var dis in dirs)
                CopyDirAllFile(dis, newPath + $"\\{dis.Name}");
        }
        #endregion

    }
}
