using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Worker2.ApiModel.Task;
using Worker2.Comm;
using Worker2.EntityModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Worker2.Controllers
{

    public class TaskController : BaseController
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
        public async Task<ListResultModel<TaskInfo>> GetTaskList([FromQuery] GetTaskListInModel input)
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

            return new ListResultModel<TaskInfo> { List = await list, Total = await total };
        }


        [HttpGet]
        public async Task<ListResultModel<TaskLog>> GetTaskLog([FromQuery] GetTaskLogModel input)
        {
            var query = _freesql.Select<TaskLog>().Where(x => x.TaskId == input.TaskId);

            if (input.StartTime.Year > 2000)
                query = query.Where(x => x.LogTime >= input.StartTime);

            if (input.EndTime.Year > 2000)
                query = query.Where(x => x.LogTime <= input.EndTime);

            if (input.LogLevels?.Any() ?? false)
                query = query.Where(x => input.LogLevels.Contains(x.Level));

            if (!string.IsNullOrEmpty(input.Content))
                query = query.Where(x => input.Content.Contains(x.Message));

            var total = query.CountAsync();
            var list = query.OrderByDescending(x => x.Id).Page(input.PageIndex, input.PageSize).ToListAsync();

            return new ListResultModel<TaskLog> { List = await list, Total = await total };
        }

        [HttpGet]
        public async Task<TaskInfo> GetTaskInfo([FromQuery] int id)
        {
            var query = _freesql.Select<TaskInfo>().Where(x => x.Id == id).FirstAsync();
            return await query;
        }

        [HttpGet]
        public async Task<object> GetTaskStats([FromQuery] int[] id)
        {
            if (id.Length <= 0)
                return new int[0];

            var query = _freesql.Select<TaskInfo>().Where(x => id.Contains(x.Id)).ToListAsync(s => new { s.Id, s.Stats });
            return await query;
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

        private readonly static Dictionary<TaskStats, TaskStats[]> TaskStatsMap = new Dictionary<TaskStats, TaskStats[]>()
        {
            { TaskStats.Stopped,new TaskStats[]{ TaskStats.PendingStart } },
            { TaskStats.Idle,new TaskStats[]{ TaskStats.WaitingToStop } },
            { TaskStats.Running,new TaskStats[]{ TaskStats.WaitingToStop}},
            { TaskStats.WaitingToStop,new TaskStats[]{ TaskStats.Aborting }},
            { TaskStats.Aborting,new TaskStats[]{}},
            { TaskStats.PendingStart ,new TaskStats[]{TaskStats.WaitingToStop}},
        };

        [HttpPost]
        public async Task<bool> ModifyTaskStats(ModifyTaskStats input)
        {
            var targetStatus = (TaskStats)input.TargetStatus;
            if (!TaskStatsMap.ContainsKey((TaskStats)input.TargetStatus))
                throw new Exception($"目标状态错误! 目标状态:{targetStatus.GetDescription()}不是约定的状态");

            var taskStats = (TaskStats)_freesql.Select<TaskInfo>().Where(x => x.Id == input.TaskId).First(s => s.Stats);
            if (taskStats <= 0)
                throw new Exception($"TaskId[{input.TaskId}]不存在或Task当前状态错误!");

            if (!TaskStatsMap[taskStats].Contains(targetStatus))
                throw new Exception($"当前任务状态为 [{taskStats.GetDescription()}] ,不能转到 [{targetStatus.GetDescription()}] 状态");

            var update = _freesql.Update<TaskInfo>().Set(x => new TaskInfo
            {
                Stats = (int)targetStatus
            }).Where(x => x.Id == input.TaskId);

            var operationResult = await update.ExecuteAffrowsAsync() > 0;

            if (operationResult)
                return true;

            throw new Exception("修改状态失败!");
        }


        #endregion
    }
}
