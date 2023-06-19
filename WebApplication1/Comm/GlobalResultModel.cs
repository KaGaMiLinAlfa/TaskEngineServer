using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Worker2.Comm
{
    /// <summary>
    /// 全局统一返回模型
    /// </summary>
    public class GlobalResultModel
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
    }


    public class PageModel<T>
    {
        /// <summary>
        /// 条件内容
        /// </summary>
        public T Content { get; set; }

        [BindRequired]
        /// <summary>
        /// 开始页
        /// </summary>
        public int PageIndex { get; set; }

        [BindRequired]
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 查询类型
        /// </summary>
        public int SearchKey { get; set; }

        /// <summary>
        /// 查询内容
        /// </summary>
        public string SearchVal { get; set; }
    }

    public class PageModel
    {
        [BindRequired]
        /// <summary>
        /// 开始页
        /// </summary>
        public int PageIndex { get; set; }

        [BindRequired]
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 查询类型
        /// </summary>
        public int SearchKey { get; set; }

        /// <summary>
        /// 查询内容
        /// </summary>
        public string SearchVal { get; set; }

    }
}
