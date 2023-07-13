using System;
using System.Collections.Generic;
using System.Text;

namespace Task.CodeHandle.EntityModel
{
    public class CodeHandle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 码包处理类型 1:后关联;2:前关联;
        /// </summary>
        public int CodeType { get; set; }

        /// <summary>
        /// 处理状态 1:未处理;2:正在处理;3:处理错误;4:处理完成
        /// </summary>
        public int Stats { get; set; }

        public string HandlePackPath { get; set; }
    }
}
