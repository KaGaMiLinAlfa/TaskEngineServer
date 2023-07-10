using System;
using System.Collections.Generic;
using System.Text;

namespace SDK.MiDuo.CodeService.Model
{
    public class QueryOrderApplyRequest : BasePageRequest
    {
        public QueryOrderApplyRequest()
        {
            MemberLogin = "admin";
        }

        /// <summary>
        /// MemberLogin
        /// </summary> 
        public string MemberLogin { get; set; }

        /// <summary>
        /// 企业名称
        /// </summary> 
        public string EntName { get; set; }

        /// <summary>
        /// 品牌商名称
        /// </summary> 
        public string CompanyName { get; set; }

        /// <summary>
        /// 码段
        /// </summary> 
        public string CodeSegment { get; set; }

        /// <summary>
        /// 生码批次
        /// </summary> 
        public string CodeBatch { get; set; }

        /// <summary>
        /// 创建开始时间
        /// </summary> 
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 创建结束时间
        /// </summary> 
        public DateTime? EndTime { get; set; }


        /// <summary>
        /// 码类型： 0：防伪码1：箱码2：包裹码3：防伪码V4.04：微商码5：质监局防伪码7：导购码
        /// </summary>
        public int[] CodeType { get; set; }

        /// <summary>
        ///  处理状态： 0：待生码 1：生码中 2：生码成功 3：生码失败 4：作废
        /// </summary>
        public int[] ProcessStatus { get; set; }

        /// <summary>
        /// 码格式:0：数字+字母+特殊字符 1：数字+字母 2 数字加小写字母
        /// </summary>
        public int[] CodeFormats { get; set; }

        /// <summary>
        /// 备注
        /// </summary> 
        public string Remark { get; set; }

        /// <summary>
        /// 码来源类型:（默认值为0） 0：米多代理后台；1：总管理后台 2：自助生码
        /// </summary> 
        public int[] SourceTypes { get; set; }

        /// <summary>
        /// 是否上传到腾讯云  0:否，1：是
        /// </summary> 
        public int[] IsUploadToTencents { get; set; }

        /// <summary>
        /// 码长度
        /// </summary> 
        public int CodeLen { get; set; }
        /// <summary>
        /// 生码订单 
        /// </summary> 
        public string CodeOrderNo { get; set; }
    }


    public class QueryOrderApplyResponse
    {
        /// <summary>
        /// 批次总数据
        /// </summary>
        public long CodeCount { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 查询生码列表
        /// </summary>
        public IList<QueryOrderApplyDto> Results { get; set; }

    }

    /// <summary>
    /// 生码信息
    /// </summary>
    public class QueryOrderApplyDto
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string CodeOrderNo { get; set; }

        /// <summary>
        /// 品牌商编号
        /// </summary>
        public string Memberlogin { get; set; }

        /// <summary>
        /// 处理状态： 0：待生码 1：生码中 2：生码成功 3：生码失败 4：作废
        /// </summary>
        public int ProcessStatus { get; set; }

        /// <summary>
        /// 码段
        /// </summary>
        public string CodeSegment { get; set; }

        /// <summary>
        /// 生码批次
        /// </summary>
        public string CodeBatch { get; set; }


        /// <summary>
        /// Zip密码
        /// </summary>
        public string ZipPwd { get; set; }

        /// <summary>
        /// 码文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 是否上传到腾讯云
        /// </summary>
        public int IsUploadToTencent { get; set; }

        /// <summary>
        /// 生码类型： 0：防伪码 1：箱码 2：包裹码 3：防伪码V4.0 4：微商码 5：质监局防伪码 6：溯源码 7：导购码 8：溯源万能码11终端动销码
        /// </summary>
        public int CodeType { get; set; }

        /// <summary>
        /// 批次码数量
        /// </summary>
        public Int64 CodeCount { get; set; }

        /// <summary>
        /// 防伪码格式： 0：数字+字母+特殊字符 1：数字+字母
        /// </summary>
        public int CodeFormat { get; set; }

        /// <summary>
        /// 码长度
        /// </summary>
        public int CodeLen { get; set; }


        /// <summary>
        /// 品牌商名称
        /// </summary>
        public string CompanyName { get; set; }



        /// <summary>
        /// 企业编码
        /// </summary>
        public string EntCode { get; set; }

        /// <summary>
        /// 企业名称
        /// </summary>
        public string EntName { get; set; }



        /// <summary>
        /// 物流码包装关系::1：单个包装 2：大小包装 3：大中小包装 4 :垛大中小
        /// </summary>
        public int PackgeType { get; set; }



        /// <summary>
        /// 垛标开始流水号
        /// </summary>
        public string BiggerSerialStart { get; set; }

        /// <summary>
        /// 垛标结束流水号
        /// </summary>
        public string BiggerSerialEnd { get; set; }

        /// <summary>
        /// 流水号位数
        /// </summary> 
        public int SerialNoLenth { get; set; }

        /// <summary>
        /// 大标开始流水号
        /// </summary>
        public string BigSerialStart { get; set; }

        /// <summary>
        /// 大标结束流水号
        /// </summary>
        public string BigSerialEnd { get; set; }

        /// <summary>
        /// 中标开始流水号
        /// </summary>
        public string MediumSerialStart { get; set; }

        /// <summary>
        /// 中标结束流水号
        /// </summary>
        public string MediumSerialEnd { get; set; }

        /// <summary>
        /// 小标开始流水号
        /// </summary>
        public string SmallSerialStart { get; set; }

        /// <summary>
        /// 小标结束流水号
        /// </summary>
        public string SmallSerialEnd { get; set; }


        /// <summary>
        /// 垛标比例:默认1
        /// </summary>
        public int BiggerPackCount { get; set; }

        /// <summary>
        /// 大标比例:1:N
        /// </summary>
        public int BigPackCount { get; set; }

        /// <summary>
        /// 中标比例:1:N
        /// </summary>
        public int MediumPackCount { get; set; }

        /// <summary>
        /// 小标比例:1:N
        /// </summary>
        public int SmallPackCount { get; set; }

        /// <summary>
		/// 开始流水号
		/// </summary> 
        public string StartSerialNo { get; set; }

        /// <summary>
        /// 结束流水号
        /// </summary> 
        public string EndSerialNo { get; set; }

        /// <summary>
        /// 批次id
        /// </summary>
        public int BatchId { get; set; }

        /// <summary>
        /// 码包数量
        /// </summary> 
        public int PackageCount { get; set; }

        /// <summary>
        /// 物流码 流水号类型:0：有序；1：乱序
        /// </summary> 
        public int SerialNoType { get; set; }

        /// <summary>
        /// 是否导入码关系
        /// </summary> 
        public int IsUpLoadCode { get; set; }

        /// <summary>
        /// 是否使用关联码：0 否 1 是（关联RelatedCodeSetting表）
        /// </summary> 
        public int HasRelatedCodeSetting { get; set; }

        /// <summary>
        /// 是否需要物流码：0 否 1 是（关联LogisticsCodeSetting表）
        /// </summary> 
        public int HasLogisticsCodeSetting { get; set; }

        /// <summary>
        /// 关联码类型:0：防伪码 1：箱码 2：包裹码 3：防伪码V4.0 4：微商码 5：质监局防伪码 7导购8万能溯源 11终端动销码
        /// </summary> 
        public int RelationCodeType { get; set; }

        /// <summary>
        /// 关联码段
        /// </summary> 
        public string RelationCodeSegment { get; set; }

        /// <summary>
        /// 关联批次
        /// </summary> 
        public string RelationCodeBatch { get; set; }

        /// <summary>
        /// 是否一码多用:0 否 1 是
        /// </summary> 
        public int IsMultiUse { get; set; }


        /// <summary>
        /// 关联码配置表 关联类型：0先关联1后关联 
        /// </summary> 
        public int RelationType { get; set; }


        /// <summary>
        /// 导购码奖项模式: 0：单奖；1：多奖
        /// </summary> 
        public int DgmPrizeType { get; set; }


        /// <summary>
        /// 父级批次ID:作为关联批次时取值
        /// </summary> 
        public int ParentBatchID { get; set; }

        /// <summary>
        /// 码来源类型:（默认值为0） 0：米多代理后台；1：总管理后台 2：自助生码
        /// </summary> 
        public int SourceType { get; set; }


        /// <summary>
        /// 关联码的子场景类型:一码多用时，表示子场景类型 0：防伪码（默认） 1：箱码 （子场景上线之前视为关联子场景－防伪码）
        /// </summary> 
        public int RelCodeSubSenceType { get; set; }

        /// <summary>
        /// 物流关联类型0：前关联 1：后关联
        /// </summary> 
        public int LogisticsRelatedType { get; set; }

        /// <summary>
        /// 类型
        /// </summary> 
        public string CodeTypeName { get; set; }

        /// <summary>
        /// 关联码之间奖项模式:0：双奖；1：单奖
        /// </summary>
        public int RelatedCodePrizeType { get; set; }
    }

}
