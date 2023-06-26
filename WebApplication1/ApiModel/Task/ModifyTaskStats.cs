using System.ComponentModel.DataAnnotations;

namespace Worker2.ApiModel.Task
{
    public class ModifyTaskStats
    {
        [Range(1, int.MaxValue, ErrorMessage = "Id不能为0或空")]
        public int TaskId { get; set; }

        /// <summary>
        /// 目标状态: 1:停止 2:待机中 3:执行中 4:等待停止 5:正在中止 6:待启动
        /// </summary>
        [Range(1, 10, ErrorMessage = "目标状态不能为0或空")]
        public int TargetStatus { get; set; }
    }
}
