using System;
using System.Collections.Generic;
using System.Text;

namespace SDK.MiDuo.CodeService.Model
{
    public class BasePageRequest
    {
        public BasePageRequest()
        {
            PageIndex = 1;
            PageSize = 10;
        }

        /// <summary>
        /// 当前页数
        /// </summary> 
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页记录数：最大每页查询1000
        /// </summary> 
        public int PageSize { get; set; }
    }
}
