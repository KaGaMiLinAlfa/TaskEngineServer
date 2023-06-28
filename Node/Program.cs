using Node.Manager;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Node
{
    public class Program
    {


        public static async Task Main(string[] args)
        {
            // Worker "D:\MiDuo\Code\TaskEngineServer\Node\bin\Debug\netcoreapp3.1\DownloadFile\V1-1" "" ""

            //ChiProcess.RunTask(@"D:\MiDuo\Code\TaskEngineServer\Node\bin\Debug\netcoreapp3.1\DownloadFile\V1-1", "", "");

            await Console.Out.WriteLineAsync($"args:{string.Join(",", args)}");
            if (args.FirstOrDefault() == "Worker")
                ChiProcess.RunTask(args[1], args[2],  args[3],"");
            else
                await new NodeManager().RunAsync();

            Console.WriteLine($"{(args.FirstOrDefault() == "Worker" ? "Worker子进程结束" : "Node结束")}");
        }
    }
}
