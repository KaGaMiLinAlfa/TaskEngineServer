using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Worker2.ApiModel.Task;
using Worker2.Comm;
using Worker2.EntityModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Worker2.Controllers
{
    [ModelValidation]
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
                query = query.Where(x => x.ClassPath.Contains(input.ClassName));

            if (input.States?.Any() ?? false)
                query = query.Where(x => input.States.Contains(x.Stats));

            var total = query.CountAsync();
            var list = query.OrderByDescending(x => x.Id).Page(input.PageIndex, input.PageSize).ToListAsync();

            return new GlobalResultModel { Data = new { Data = await list, Total = await total } };
        }


        [HttpGet]
        public async Task<GlobalResultModel> GetTaskLog([FromQuery] GetTaskLogModel input)
        {
            var query = _freesql.Select<TaskLog>().Where(x => x.TaskId == input.TaskId);

            var total = query.CountAsync();
            var list = query.OrderByDescending(x => x.Id).Page(input.PageIndex, input.PageSize).ToListAsync();

            return new GlobalResultModel { Data = new { Data = await list, Total = await total } };
        }

        [HttpGet]
        public async Task<GlobalResultModel> GetTaskInfo([FromQuery] int id)
        {
            var query = _freesql.Select<TaskInfo>().Where(x => x.Id == id).FirstAsync();

            return new GlobalResultModel { Data = await query };
        }


        #endregion

        #region Update

        [HttpPost]
        public async Task<GlobalResultModel> CreateTask(CreateTaskModel input)
        {
            if (string.IsNullOrEmpty(input.Cron))
                input.Cron = string.Empty;

            var insert = _freesql.Insert(new TaskInfo
            {
                TaskName = input.TaskName,
                DllName = input.DllName,
                Cron = input.Cron,
                ClassPath = input.ClassPath,
                PackageId = input.PackageId,

                Config = string.Empty,
                Stats = 1,
            });

            return new GlobalResultModel { Data = await insert.ExecuteIdentityAsync() };
        }

        [HttpPost]
        public async Task<GlobalResultModel> ModifyTask(ModifyTaskModel input)
        {
            var update = _freesql.Update<TaskInfo>().Set(x => new TaskInfo
            {
                TaskName = input.TaskName,
                DllName = input.DllName,
                Cron = input.Cron,
                ClassPath = input.ClassPath,
                PackageId = input.PackageId,

            }).Where(x => x.Id == input.Id);

            return new GlobalResultModel { Data = await update.ExecuteAffrowsAsync() > 0 };
        }

        #endregion
    }
}
