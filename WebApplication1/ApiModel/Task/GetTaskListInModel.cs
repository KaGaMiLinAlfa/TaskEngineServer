using Worker2.Comm;

namespace Worker2.ApiModel.Task
{
    public class GetTaskListInModel: PageModel
    {
        /// <summary>
        /// Task名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 执行类名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Task状态
        /// </summary>
        public int State { get; set; }

    }
}
