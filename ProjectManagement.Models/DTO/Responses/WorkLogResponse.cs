using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Models.DTO.Responses
{
    public class WorkLogResponse
    {
        public int Id { get; set; }
        public int TimeSpent { get; set; }
        public string Description { get; set; }
        public int TaskWorkedOnId { get; set; }
        public string TaskWorkOnName { get; set; }
    }
}
