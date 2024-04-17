using System;

namespace Worker2.EntityModel
{
    public class MessageInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Receiver { get; set; }
        public string Sender { get; set; }
        public int Stats { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        
    }
}