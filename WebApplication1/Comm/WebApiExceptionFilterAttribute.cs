using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using System.Text;
using System;

namespace Worker2.Comm
{
    public class WebApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// 重写基类的异常处理方法
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnException(ExceptionContext actionExecutedContext)
        {
            Exception exception = actionExecutedContext.Exception;
            string url = actionExecutedContext.HttpContext.Request.Scheme + "://" + actionExecutedContext.HttpContext.Request.Host.Value + actionExecutedContext.HttpContext.Request.Path.Value;
            string queryStr = "";
            string method = actionExecutedContext.HttpContext.Request.Method.ToLower();
            if (method == "get")
            {
                url += actionExecutedContext.HttpContext.Request.QueryString.Value;
            }
            else if (method == "post")
            {
                try
                {
                    actionExecutedContext.HttpContext.Request.Body.Position = 0;
                    var stream = actionExecutedContext.HttpContext.Request.Body;
                    long? length = actionExecutedContext.HttpContext.Request.ContentLength;
                    if (length != null && length > 0)
                    {
                        // 使用这个方式读取，并且使用异步
                        StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
                        queryStr = streamReader.ReadToEndAsync().GetAwaiter().GetResult();
                    }
                    actionExecutedContext.HttpContext.Request.Body.Position = 0;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("读取接口请求参数错误：" + ex.Message);
                }
            }
            var response = new GlobalResultModel();
            response.Code = 500;
            response.Msg = actionExecutedContext.Exception.Message;

            actionExecutedContext.Result = new JsonResult(response);
            base.OnException(actionExecutedContext);
        }
    }

}
