using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worker2.EntityModel
{
    public class HeartbeatLog
    {
        [Key]
        public int ID { get; set; }

        public string Group { get; set; }
        public string SiteName { get; set; }

        public DateTime CreateTime { get; set; }
        public sbyte Count { get; set; }
        
    }
}
