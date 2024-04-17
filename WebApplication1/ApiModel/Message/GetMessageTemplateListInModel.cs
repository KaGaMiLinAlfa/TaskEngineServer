namespace Worker.ApiModel.Message
{
    public class GetMessageTemplateListInModel
    {
        public string TemplateName { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        
    }
}