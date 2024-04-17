using System;
using System.ComponentModel.DataAnnotations;

namespace Worker2.EntityModel
{
    public class MessageTemplate
    {
        [Key]
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string TemplateContent { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int Stats { get; set; }
        
    }
}