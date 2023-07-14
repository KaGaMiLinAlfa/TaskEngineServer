using System.ComponentModel.DataAnnotations;
using System;

namespace Worker2.EntityModel
{
    public class CodeHandleLog
    {
        [Key]
        public long Id { get; set; }

        public int HandleId { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(255)]
        public string Message { get; set; }
    }
}
