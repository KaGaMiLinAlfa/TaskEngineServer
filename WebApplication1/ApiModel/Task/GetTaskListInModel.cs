namespace Worker2.ApiModel.Task
{
    public class GetTaskListInModel
    {
        /// <summary>
        /// Task名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// Task状态
        /// </summary>
        public int State { get; set; }
    }
}
