using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Worker2.Comm
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class ModelValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var isvalid = context.ModelState.IsValid;
            if (!isvalid)
            {
                string error = string.Empty;
                var modelState = context.ModelState;
                foreach (var key in modelState.Keys)
                {
                    var state = modelState[key];
                    if (state.Errors.Any())
                    {
                        error = state.Errors.First().ErrorMessage == "" ? key + "参数数据格式不合理请检查！" : state.Errors.First().ErrorMessage;
                        break;
                    }
                }

                var response = new GlobalResultModel() { Code = 500, Msg = error };

                context.Result = new JsonResult(response) { StatusCode = 500 };

                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
