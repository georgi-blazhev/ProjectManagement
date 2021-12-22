using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Models.DTO.Requests.TaskRequests
{
    public class CreateTask
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public string AssigneeUserName { get; set; }
    }
}
