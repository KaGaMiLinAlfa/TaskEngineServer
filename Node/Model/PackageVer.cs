using System;
using System.ComponentModel.DataAnnotations;

namespace Node.Model
{
    public class TaskPackageInfo
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public int Category { get; set; }
        public string PackageUrl { get; set; }

        public string Remark { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
