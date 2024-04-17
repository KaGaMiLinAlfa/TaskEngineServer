using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Worker.ApiModel.Message;
using Worker2.Comm;
using Worker2.EntityModel;

namespace Worker2.Controllers
{
    public class MessageController : BaseController
    {
        private readonly IFreeSql _freesql;

        public MessageController(IFreeSql freeSql)
        {
            _freesql = freeSql;
        }


        #region 消息通知模板
        
        [HttpGet]
        public async Task<ListResultModel<MessageTemplate>> GetMessageTemplateList(GetMessageTemplateListInModel input)
        {
            var query = _freesql.Select<MessageTemplate>();

            if (!string.IsNullOrEmpty(input?.TemplateName))
                query = query.Where(x => x.TemplateName.Contains(input.TemplateName));

            var total = query.CountAsync();
            var list = query.OrderByDescending(x => x.Id).Page(input.PageIndex, input.PageSize).ToListAsync();

            return new ListResultModel<MessageTemplate> { List = await list, Total = await total };
        }
        
        //添加消息模板
        [HttpPost]
        public async Task<MessageTemplate> AddMessageTemplate([FromBody] AddMessageTemplateInModel input)
        {
            var entity = new MessageTemplate
            {
                TemplateName = input.TemplateName,
                TemplateContent = input.TemplateContent,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            await _freesql.Insert(entity).ExecuteAffrowsAsync();
            return entity;
        }
        
        //修改消息模板状态
        [HttpPost]
        public async Task<MessageTemplate> UpdateMessageTemplate([FromBody] UpdateMessageTemplateInModel input)
        {
            var entity = await _freesql.Select<MessageTemplate>().Where(x => x.Id == input.Id).FirstAsync();
            if (entity == null)
                throw new Exception("消息模板不存在");
            
            entity.TemplateName = input.TemplateName;
            entity.TemplateContent = input.TemplateContent;
            entity.UpdateTime = DateTime.Now;
            entity.Stats = input.Stats;
            
            await _freesql.Update<MessageTemplate>().SetSource(entity).ExecuteAffrowsAsync();
            return entity;
        }

        #endregion

        #region 消息通知
        
        [HttpGet]
        public async Task<ListResultModel<MessageInfo>> GetMessageList([FromQuery] GetMessageListInModel input)
        {
            var query = _freesql.Select<MessageInfo>();

            if (!string.IsNullOrEmpty(input.Receiver))
                query = query.Where(x => x.Receiver == input.Receiver);

            if (!string.IsNullOrEmpty(input.Title))
                query = query.Where(x => x.Title == input.Title);

            if (input.SendTimeStart.Year > 2000)
                query = query.Where(x => x.CreateTime >= input.SendTimeStart);

            if (input.SendTimeEnd.Year > 2000)
                query = query.Where(x => x.CreateTime <= input.SendTimeEnd);

            var total = query.CountAsync();
            var list = query.OrderByDescending(x => x.Id).Page(input.PageIndex, input.PageSize).ToListAsync();

            return new ListResultModel<MessageInfo> { List = await list, Total = await total };
        }
        
        //修改消息状态
        [HttpPost]
        public async Task<MessageInfo> UpdateMessage([FromBody] UpdateMessageInModel input)
        {
            var entity = await _freesql.Select<MessageInfo>().Where(x => x.Id == input.Id).FirstAsync();
            if (entity == null)
                throw new Exception("消息不存在");
            
            entity.Stats = input.Stats;
            entity.UpdateTime = DateTime.Now;
            
            await _freesql.Update<MessageInfo>().SetSource(entity).ExecuteAffrowsAsync();
            return entity;
        }
        
        #endregion
    }
}