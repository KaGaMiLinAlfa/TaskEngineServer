using System.Data;
using System;

namespace TaskEngine.Model
{
    public class TaskInfo
    {
        public int Id { get; private set; }
        public string Status { get; private set; }
        public string DLLFilePath { get; private set; }
        public string ClassName { get; private set; }


    }

}
