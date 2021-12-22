using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Models.DTO.Responses
{
    public class TasksResponse
    {
        public string Name { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string AssigneeName { get; set; }
        public string Status { get; set; }
    }
}
