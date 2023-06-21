using System.ComponentModel.DataAnnotations;
using System;

namespace Worker2.EntityModel
{
    public class TaskLog
    {
        [Key]
        public long Id { get; set; }

        public int TaskId { get; set; }

        public int NodeId { get; set; }

        public DateTime LogTime { get; set; } = DateTime.Now;

        public int Level { get; set; }

        [Required]
        [MaxLength(255)]
        public string Message { get; set; }
    }
}
