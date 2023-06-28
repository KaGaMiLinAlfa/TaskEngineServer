
using System;

namespace BaseTaskManager
{
    public abstract class BaseTask
    {
        public int TaskId { get; set; }
        public virtual void Init()
        {

            Environment.SetEnvironmentVariable("TaskId", TaskId.ToString());

            Console.WriteLine("BaseTask初始化完毕");
        }
        public abstract void Run();
    }
}
