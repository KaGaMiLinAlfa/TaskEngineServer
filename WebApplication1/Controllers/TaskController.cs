using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Worker2.ApiModel.Task;
using Worker2.Comm;
using Worker2.EntityModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Worker2.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TaskController : ControllerBase
    {
        private readonly IFreeSql _freesql;
        public TaskController(IFreeSql freeSql)
        {
            _freesql = freeSql;
        }

        #region Query

        /// <summary>
        /// 分页查询Task列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<GlobalResultModel> GetTaskList([FromQuery] PageModel<GetTaskListInModel> input)
        {
            var query = _freesql.Select<TaskInfo>();

            //if (!string.IsNullOrEmpty(input?.))
            //    query = query.Where(x => x.TaskName.Contains(taskName));

            //if (input.Content?.State > 0)
            //    query = query.Where(x => x.State == input.Content.State);

            var list = query.ToListAsync();
            var total = query.CountAsync();

            return new GlobalResultModel { Data = new { Data = await list, Total = await total } };
        }







        #endregion
    }
}
