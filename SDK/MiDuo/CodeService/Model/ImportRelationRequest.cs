using System;
using System.Collections.Generic;
using System.Text;

namespace SDK.MiDuo.CodeService.Model
{
    public class ImportRelationRequest
    {

        /// <summary>
        /// 文件路径
        /// </summary> 
        public string Path { get; set; }


        /// <summary>
        /// 数量
        /// </summary> 
        public int dateCount { get; set; }


        /// <summary>
        /// 码类型  -1为不关联,0 一级标签,1 二级标签（大小包装） 2 三级标签（大中小包装） 3 后关联 4 四级标签（垛大中小标）
        /// </summary> 
        public int codeType { get; set; }


        /// <summary>
        /// 码段类型
        /// </summary> 
        public int type { get; set; }


        /// <summary>
        /// 码批次ID
        /// </summary> 
        public int id { get; set; }


        /// <summary>
        /// 品牌商编码
        /// </summary>
        public string Memberlogin { get; set; }

    }

    public class ImportFangcuanRelationResponse
    {
        public int sucCount { get; set; }
        public int errCount { get; set; }
    }

}
