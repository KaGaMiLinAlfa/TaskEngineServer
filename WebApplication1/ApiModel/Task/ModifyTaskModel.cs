using System.ComponentModel.DataAnnotations;

namespace Worker2.ApiModel.Task
{
    public class ModifyTaskModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Id不能为0或空")]
        public int Id { get; set; }

        [Required(ErrorMessage = "任务名称不能为空")]
        public string TaskName { get; set; }
        public string Cron { get; set; }

        [Required(ErrorMessage = "Dll名称不能为空")]
        public string DllName { get; set; }

        [Required(ErrorMessage = "类路径不能为空")]
        public string ClassPath { get; set; }
        public int PackageId { get; set; }
    }
}
