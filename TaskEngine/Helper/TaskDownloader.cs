using System.IO;
using System.Net;
using TaskEngine.Model;

namespace TaskEngine.Helper
{
    public class TaskDownloader
    {
        private string _downloadDirectory;

        public TaskDownloader(string downloadDirectory)
        {
            _downloadDirectory = downloadDirectory;
        }

        public string DownloadDll(TaskInfo task)
        {
            string localFilePath = Path.Combine(_downloadDirectory, Path.GetFileName(task.DLLFilePath));
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(task.DLLFilePath, localFilePath);
            }
            return localFilePath;
        }
    }
}
