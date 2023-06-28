using BaseTaskManager;
using LogManager;
using System;
using System.Diagnostics;

namespace tasktest
{
    public class Class1 : BaseTask
    {
 

        public override void Run()
        {
            try
            {
                Console.WriteLine($"PID：{Process.GetCurrentProcess().Id}");
                LogHelper.Debug("Task测试", null);
                System.Threading.Thread.Sleep(10 * 1000);
                Console.WriteLine("Task结束--------------over");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"子进程异常:{ex.Message}--------------over", ex);
            }

        }
    }
}
