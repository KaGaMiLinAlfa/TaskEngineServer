using System;

namespace Worker.ApiModel.Message
{
    public class GetMessageListInModel
    {
        public string Title { get; set; }
        public string Receiver { get; set; }
        
        public DateTime SendTimeStart { get; set; }
        public DateTime SendTimeEnd { get; set; }
        
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        
    }
}