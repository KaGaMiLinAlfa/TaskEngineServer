using System;
using System.Collections.Generic;
using System.Text;

namespace SDK.MiDuo.CodeService.Model
{
    public class BaseResponseModel<T>
    {
        /// <summary>
        /// 返回状态码
        /// </summary>
        public int Return_code { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Return_msg { get; set; }
        /// <summary>
        /// 返回集合
        /// </summary>
        public T Return_data { get; set; }
    }

    public class BaseResponseModelV1<T>
    {
        /// <summary>
        /// 返回状态码
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 返回集合
        /// </summary>
        public T parameter { get; set; }
    }
}
