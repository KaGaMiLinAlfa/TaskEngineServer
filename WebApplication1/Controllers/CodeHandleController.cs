using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Worker2.ApiModel.CodeHandle;
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
        public async Task<GlobalResultModel> GetCodeHandleList()
        {
            var list = _freesql.Select<CodeHandle>().ToListAsync();
            var total = _freesql.Select<CodeHandle>().CountAsync();

            return new GlobalResultModel { Code = 200, Data = new ListResultModel<CodeHandle> { List = await list, Total = await total } };
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
                HandlePackPath = input.HandlePackPath
            }).ExecuteIdentityAsync();


            return await id;
        }


        #endregion


        #region Other

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");

            var uploadsFolderPath = Path.Combine(AppContext.BaseDirectory, "uploads");
            if (!Directory.Exists(uploadsFolderPath))
                Directory.CreateDirectory(uploadsFolderPath);


            var filePath = Path.Combine(uploadsFolderPath, $"{DateTime.Now:yyyyMMdd-HHmmss}-{file.FileName}");
            using (var fileStream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(fileStream);

            return Ok("File uploaded successfully.");
        }

        #endregion

    }
}
