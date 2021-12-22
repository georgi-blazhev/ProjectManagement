using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Models.DTO.Requests.TaskRequests
{
    public class EditTask
    {
        [Required]
        public string Name { get; set; }
        public string Status { get; set; }
    }
}
