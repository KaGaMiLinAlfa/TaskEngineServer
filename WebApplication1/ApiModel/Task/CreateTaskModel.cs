using System.ComponentModel.DataAnnotations;

namespace Worker2.ApiModel.Task
{
    public class CreateTaskModel
    {
        [Required(ErrorMessage = "任务名称不能为空")]
        [MaxLength(50, ErrorMessage = "任务名称长度不能超过50")]
        public string TaskName { get; set; }


        public string Cron { get; set; }

        [Required(ErrorMessage = "Dll名称不能为空")]
        [MaxLength(50, ErrorMessage = "Dll名称长度不能超过50")]
        public string DllName { get; set; }

        [Required(ErrorMessage = "类路径不能为空")]
        [MaxLength(50, ErrorMessage = "类路径长度不能超过50")]
        public string ClassPath { get; set; }

        public int PackageId { get; set; }

    }
}
