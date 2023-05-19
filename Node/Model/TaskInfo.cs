using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Model
{
    public class TaskInfo
    {
        public int Id { get; set; }

        public string TaskId { get; set; }

        public string NodeId { get; set; }

        public string Name { get; set; }

        public string Cron { get; set; }

        public string Config { get; set; }

        public string DllName { get; set; }

        public string ClassPath { get; set; }

        public string PackageUrl { get; set; }

        public int PackageId { get; set; }

        public bool IsRun { get; set; }
    }
}
