namespace Worker.ApiModel.Message
{
    public class UpdateMessageTemplateInModel
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string TemplateContent { get; set; }
        public sbyte Stats { get; set; }
        
    }
}