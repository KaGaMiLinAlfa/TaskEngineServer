using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Worker2.ApiModel.CodeHandle;
using Worker2.ApiModel.Task;
using Worker2.Comm;
using Worker2.EntityModel;

namespace Worker2.Controllers
{
    public class CodeHandleController : BaseController
    {
        private readonly IFreeSql _freesql;
        public CodeHandleController(IFreeSql freeSql)
        {
            _freesql = freeSql;
        }




        #region Query

        [HttpGet]
        public async Task<ListResultModel<CodeHandle>> GetCodeHandleList([FromQuery] GetTaskListInModel input)
        {
            var query = _freesql.Select<CodeHandle>();


            var list = query.OrderByDescending(x => x.Id).Page(input.PageIndex, input.PageSize).ToListAsync();
            var total = query.CountAsync();

            return new ListResultModel<CodeHandle> { List = await list, Total = await total };
        }


        [HttpGet]
        public async Task<CodeHandle> GetCodeHandleInfo([FromQuery] int id)
        {
            var query = _freesql.Select<CodeHandle>().Where(x => x.Id == id).FirstAsync();

            return await query;
        }

        [HttpGet]
        public async Task<List<CodeHandleLog>> GetCodeHandleLog([FromQuery] int id)
        {
            var query = _freesql.Select<CodeHandleLog>().Where(x => x.HandleId == id);


            var total = query.CountAsync();
            var list = query.OrderBy(x => x.Id).ToListAsync();

            return await list;
        }


        #endregion

        #region Update

        [HttpPost]
        public async Task<long> CreateCodeHandle(CreateCodeHandleModel input)
        {

            var id = _freesql.Insert(new CodeHandle
            {
                Name = input.Name,
                CreateTime = DateTime.Now,
                CodeType = input.CodeType,
                Stats = 1,
                HandlePackPath = input.HandlePackPath,
                Remark = string.Empty,
                ErrorFilePath = string.Empty,
            }).ExecuteIdentityAsync();


            return await id;
        }

        [HttpPost]
        public async Task<long> ModifyCodeHandle(CreateCodeHandleModel input)
        {

            var updateCount = _freesql.Update<CodeHandle>().Set(x => new CodeHandle
            {
                Name = input.Name,
                CodeType = input.CodeType,
                Stats = 1,
                HandlePackPath = input.HandlePackPath,
                Remark = input.Remark
            }).Where(x => x.Id == input.Id).ExecuteAffrowsAsync();


            return await updateCount;
        }


        #endregion


        #region Other

        [HttpPost]
        public async Task<GlobalResultModel<string>> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("获取文件失败");

            var uploadsFolderPath = Path.Combine(AppContext.BaseDirectory, "uploads");
            if (!Directory.Exists(uploadsFolderPath))
                Directory.CreateDirectory(uploadsFolderPath);

            var filePath = Path.Combine(uploadsFolderPath, $"{DateTime.Now:yyyyMMdd-HHmmss}-{file.FileName}");
            using (var fileStream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(fileStream);

            return filePath;
        }

        #endregion

    }
}
