using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Models.DTO.Requests.WorkLogRequests
{
    public class CreateWorkLog
    {
        [Required]
        [Range(0, 8)]
        public int TimeSpent { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int TaskWorkedOnId { get; set; }
    }
}

