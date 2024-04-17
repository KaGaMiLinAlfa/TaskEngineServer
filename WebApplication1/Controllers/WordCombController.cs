using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Worker2.Comm;
using Worker2.EntityModel;
using Worker2.ApiModel.WordComb;
using System.Linq;

namespace Worker2.Controllers
{
    public class WordCombController : BaseController
    {
        private readonly IFreeSql _freesql;

        public WordCombController(IFreeSql freeSql)
        {
            _freesql = freeSql;
        }


        #region Query

        [HttpGet]
        public async Task<List<WordCombGroup>> GetWordGroupList()
        {
            var list = await _freesql.Select<WordCombGroup>().ToListAsync();
            return list;
        }


        [HttpGet]
        public async Task<List<string>> GetParamNameList()
        {
            var list = await _freesql.Select<WordCombGroup>().Distinct().ToListAsync(s => s.ParamName);
            return list;
        }

        //[HttpPost]
        //public async Task<List<WordCombParams>> GetWordCombParamsList()
        //{
        //    var list = await _freesql.Select<WordCombParams>().ToListAsync();
        //    return list;
        //}

        [HttpGet]
        public async Task<List<WordCombContent>> GetWordCombContentList()
        {
            var list = await _freesql.Select<WordCombContent>().ToListAsync();
            return list;
        }

        #endregion

        #region Update

        [HttpPost]
        public async Task<bool> CreateWordGroup(CreateWordCombGroupModel input)
        {
            //获取分组是否存在
            var isexist = await _freesql.Select<WordCombGroup>().Where(x => x.GroupName == input.GroupName).AnyAsync();

            if (isexist)
                throw new Exception($"分组名 {input.GroupName} 已存在");

            var groups = input.Params.Select(s => new WordCombGroup
            {
                GroupName = input.GroupName,
                ParamName = s.Key,
                ParamVal = s.Value,
            }).ToList();

            var issucceed = _freesql.Insert(groups).ExecuteAffrowsAsync();
            return await issucceed == groups.Count;
        }

        //[HttpPost]
        //public async Task<bool> CreateWordParams(CreateWordCombParamsModel input)
        //{
        //    //判断参数名是否已存在
        //    var isexist = await _freesql.Select<WordCombParams>().Where(x => x.ParamName == input.ParamName).AnyAsync();
        //    if (isexist)
        //        throw new Exception($"参数名 {input.ParamName} 已存在");

        //    var param = new WordCombParams
        //    {
        //        ParamName = input.ParamName,
        //        ReplaceStr = input.ReplaceStr,
        //    };

        //    var issucceed = _freesql.Insert(new WordCombParams
        //    {
        //        ParamName = input.ParamName,
        //        ReplaceStr = input.ReplaceStr,
        //    }).ExecuteAffrowsAsync();

        //    return await issucceed > 0;
        //}

        //创建WordCombContent
        [HttpPost]
        public async Task<bool> CreateWordCombContent(CreateWordCombContentModel input)
        {
            //获取分组是否存在
            var isexist = await _freesql.Select<WordCombContent>().Where(x => x.Title == input.Title).AnyAsync();

            if (isexist)
                throw new Exception($"正文名 {input.Title} 已存在");


            var issucceed = _freesql.Insert(new WordCombContent
            {
                Title = input.Title,
                WordContent = input.WordContent,
            }).ExecuteAffrowsAsync();
            return await issucceed > 0;
        }

        //编辑分组
        [HttpPost]
        public async Task<bool> EditWordGroup(CreateWordCombGroupModel input)
        {
            var group = await _freesql.Select<WordCombGroup>().Where(x => x.GroupName == input.GroupName).ToOneAsync();
            if (group == null)
                throw new Exception($"分组不存在");

            //删除原有分组
            await _freesql.Delete<WordCombGroup>().Where(x => x.GroupName == input.GroupName).ExecuteAffrowsAsync();

            //添加新分组
            var groups = input.Params.Select(s => new WordCombGroup
            {
                GroupName = input.GroupName,
                ParamName = s.Key,
                ParamVal = s.Value,
            }).ToList();

            var issucceed = _freesql.Insert(groups).ExecuteAffrowsAsync();

            return await issucceed == groups.Count;
        }

        //编辑参数
        //[HttpPost]
        //public async Task<bool> EditWordParams(CreateWordCombParamsModel input)
        //{
        //    var param = await _freesql.Select<WordCombParams>().Where(x => x.ParamName == input.ParamName).ToOneAsync();
        //    if (param == null)
        //        throw new Exception($"参数不存在");

        //    param.ReplaceStr = input.ReplaceStr;

        //    var issucceed = _freesql.Update<WordCombParams>().SetSource(param).ExecuteAffrowsAsync();

        //    return await issucceed > 0;
        //}

        //编辑WordCombContent
        [HttpPost]
        public async Task<bool> EditWordCombContent(CreateWordCombContentModel input)
        {
            var content = await _freesql.Select<WordCombContent>().Where(x => x.Title == input.Title).ToOneAsync();
            if (content == null)
                throw new Exception($"正文不存在");

            content.WordContent = input.WordContent;

            var issucceed = _freesql.Update<WordCombContent>().SetSource(content).ExecuteAffrowsAsync();

            return await issucceed > 0;
        }

        [HttpPost]
        public async Task<bool> DeleteWordGroup(string groupname)
        {
            var issucceed = _freesql.Delete<WordCombGroup>().Where(x => x.GroupName == groupname).ExecuteAffrowsAsync();
            return await issucceed > 0;
        }

        //[HttpPost]
        //public async Task<bool> DeleteWordParams(string paramname)
        //{
        //    var issucceed = _freesql.Delete<WordCombParams>().Where(x => x.ParamName == paramname).ExecuteAffrowsAsync();
        //    return await issucceed > 0;
        //}

        [HttpPost]
        public async Task<bool> DeleteWordCombContent(string title)
        {
            var succeed = _freesql.Delete<WordCombContent>().Where(x => x.Title == title).ExecuteAffrowsAsync();
            return await succeed > 0;
        }

        #endregion
    }
}