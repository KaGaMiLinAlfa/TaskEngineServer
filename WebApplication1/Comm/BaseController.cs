using Microsoft.AspNetCore.Mvc;

namespace Worker2.Comm
{
    [ModelValidation]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BaseController : ControllerBase
    {

        protected ActionResult<T> Success<T>(T data) => Ok(new GlobalResultModel { Data = data, Message = "请求成功" });

        protected ActionResult ParameterError(string msg) => StatusCode(400, new GlobalResultModel { Code = 400, Message = msg });

        protected ActionResult OperationError(string msg) => StatusCode(409, new GlobalResultModel { Code = 409, Message = msg });



    }
}
