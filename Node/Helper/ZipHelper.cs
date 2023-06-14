using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Ionic.Zip;

namespace Node.Helper
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

            var zipfile = Path.Combine(savePath, Path.GetFileName(url));
            if (File.Exists(zipfile))
                File.Delete(zipfile);

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
