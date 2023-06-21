
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using Worker2.Comm;

namespace Worker2.ApiModel.Task
{
    public class GetTaskLogModel : PageModel
    {
        [Required]
        public int TaskId { get; set; }
    }
}
