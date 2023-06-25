using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;

namespace Worker2.Comm
{
    public class WebApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// 重写基类的异常处理方法
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override async Task OnExceptionAsync(ExceptionContext actionExecutedContext)
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
                    var request = actionExecutedContext.HttpContext.Request;

                    var requestBody = "";
                    using (var reader = new StreamReader(request.Body))
                        requestBody = await reader.ReadToEndAsync();
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("读取接口请求参数错误：" + ex.Message);
                }
            }
            var response = new GlobalResultModel();
            response.Code = 500;
            response.Msg = actionExecutedContext.Exception.Message;


            actionExecutedContext.Result = new JsonResult(response) { StatusCode = 500 };
            base.OnException(actionExecutedContext);
        }
    }

}
