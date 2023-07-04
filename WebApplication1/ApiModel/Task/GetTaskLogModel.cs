
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using Worker2.Comm;

namespace Worker2.ApiModel.Task
{
    public class GetTaskLogModel : PageModel
    {
        [Required]
        public int TaskId { get; set; }

        /// <summary>
        /// 日志开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 日志结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        public int[] LogLevels { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content { get; set; }
    }
}
