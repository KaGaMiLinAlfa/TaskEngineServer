using System;

namespace Worker2.ApiModel.CodeHandle
{
    public class CreateCodeHandleModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CodeType { get; set; }

        /// <summary>
        /// 处理状态 1:未处理;2:正在处理;3:处理错误;4:处理完成
        /// </summary>
        public int Stats { get; set; }

        public string HandlePackPath { get; set; }
    }
}
