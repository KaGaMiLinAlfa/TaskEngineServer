using FreeSql.DataAnnotations;

namespace Worker2.EntityModel
{
    public class TaskInfo
    {
        /// <summary>
        /// Task主键
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }

        [Column(IsNullable = false, StringLength = 32)]
        public string TaskId { get; set; }

        [Column(IsNullable = false, StringLength = 32)]
        public string NodeId { get; set; }

        [Column(IsNullable = false, StringLength = 128)]
        public string TaskName { get; set; }

        [Column(IsNullable = false, StringLength = 32)]
        public string Cron { get; set; }

        [Column(IsNullable = false, StringLength = 32)]
        public string Config { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [Column(IsNullable = false, StringLength = 128)]
        public string DllName { get; set; }

        /// <summary>
        /// 执行类完整路径
        /// </summary>
        [Column(IsNullable = false, StringLength = 255)]
        public string ClassPath { get; set; }

        /// <summary>
        /// ftp上传包地址
        /// </summary>
        [Column(IsNullable = false, StringLength = 255)]
        public string PackageUrl { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        [Column(IsNullable = false)]
        public int PackageId { get; set; }

        /// <summary>
        /// 运行状态 1:停止 2:待机中 3:执行中 4:等待停止 5:正在中止 
        /// </summary>
        [Column(IsNullable = false)]
        public int Stats { get; set; }
    }

}
