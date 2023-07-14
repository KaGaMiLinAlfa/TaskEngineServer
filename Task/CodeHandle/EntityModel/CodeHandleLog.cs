using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Task.CodeHandle.EntityModel
{
    public class CodeHandleLog
    {
        [Key]
        public int Id { get; set; }

        public int HandleId { get; set; }

        public DateTime CreateTime { get; set; }

        public int Type { get; set; }

        public string Message { get; set; }
    }
}
