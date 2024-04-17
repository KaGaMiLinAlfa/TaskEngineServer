using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Worker2.ApiModel.Heartbeat;
using Worker2.Comm;
using Worker2.EntityModel;

namespace Worker2.Controllers
{
    public class HeartbeatController : BaseController
    {
        private readonly IFreeSql _freesql;
        public HeartbeatController(IFreeSql freeSql)
        {
            _freesql = freeSql;
        }

        [HttpGet]
        public Task<string> Heartbeat(string group, string siteName)
        {
            var query = _freesql.Select<HeartbeatLog>();

            var now = DateTime.Now;
            var nowWithoutSeconds = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

            var model = query.Where(x => x.Group == group && x.SiteName == siteName && x.CreateTime >= nowWithoutSeconds)
                            .OrderByDescending(x => x.ID).First();

            if (model == null)
                _freesql.Insert(new HeartbeatLog
                {
                    Group = group,
                    SiteName = siteName,
                    CreateTime = DateTime.Now,
                    Count = 1
                }).ExecuteAffrows();
            else
                _freesql.Update<HeartbeatLog>(model.ID).Set(x => new HeartbeatLog { Count = (sbyte)(x.Count + 1) }).ExecuteAffrows();

            return Task.FromResult("OK");
        }

        [HttpGet]
        public Task<List<SiteLastHeartbeat>> SiteHeartbeatList()
        {
            //查询每个站点最近一次心跳时间
            var result = _freesql.Select<HeartbeatLog>()
                                .GroupBy(x => x.SiteName)
                                .OrderByDescending(x => x.Key)
                                .ToList(s => new SiteLastHeartbeat
                                {
                                    SiteName = s.Key,
                                    LastTime = s.Max(s.Value.CreateTime),
                                    Group = s.Min(s.Value.Group)
                                });

            return Task.FromResult(result);

        }


    }
}
