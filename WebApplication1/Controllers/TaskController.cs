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

        [HttpGet]
        public string test(string val) => val;

        /// <summary>
        /// 分页查询Task列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<GlobalResultModel> GetTaskList([FromQuery] GetTaskListInModel input)
        {
            var query = _freesql.Select<TaskInfo>();

            if (!string.IsNullOrEmpty(input?.TaskName))
                query = query.Where(x => x.TaskName.Contains(input.TaskName));

            if (!string.IsNullOrEmpty(input?.ClassName))
                query = query.Where(x => x.TaskName.EndsWith(input.TaskName) || x.ClassPath == input.TaskName);

            if (input.State > 0)
                query = query.Where(x => x.Stats == input.State);

            var total = query.CountAsync();
            var list = query.OrderByDescending(x => x.Id).Page(input.PageIndex, input.PageSize).ToListAsync();
            
            return new GlobalResultModel { Data = new { Data = await list, Total = await total } };
        }







        #endregion
    }
}
