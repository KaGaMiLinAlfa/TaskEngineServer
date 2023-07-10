using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDK.MiDuo.CodeService.Model
{
    public class ValidateCodeResponse
    {
        /// <summary>
        /// 状态码: 1,--状态码:0 必要参数为空,1 正确，首次,2 重复,3 错误，不存在,5 系统维护,7 接口秘钥错误
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 首次查询时间
        /// </summary>
        [JsonProperty(PropertyName = "first")]
        public string FirstQueryTime { get; set; }

        /// <summary>
        /// 当前查询次数
        /// </summary>
        [JsonProperty(PropertyName = "num")]
        public int QueryCount { get; set; }

        /// <summary>
        /// 产品ID可无视
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 码段
        /// </summary>
        public string CodeSegment { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        public string CodeBatch { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        [JsonProperty(PropertyName = "ID")]
        public string SerialNo { get; set; }

        /// <summary>
        /// 垛标流水号
        /// </summary>
        [JsonProperty(PropertyName = "tid")]
        public string BiggerNo { get; set; }

        /// <summary>
        /// 大标流水号
        /// </summary>
        [JsonProperty(PropertyName = "bid")]
        public string BigNo { get; set; }

        /// <summary>
        /// 中标流水号
        /// </summary>
        [JsonProperty(PropertyName = "mid")]
        public string MediumNo { get; set; }

        /// <summary>
        /// 小标流水号
        /// </summary>
        [JsonProperty(PropertyName = "sid")]
        public string SmallNo { get; set; }

        /// <summary>
        /// 品牌商编号
        /// </summary>
        public string MemberLogin { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string ValidateCode { get; set; }
    }
}
