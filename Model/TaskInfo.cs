using DAO.DQDbContext;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model
{
    public class TaskInfo : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public string TaskName { get; set; }

    }
}
