using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.DAL.Entities
{
    public class Task : AbstractEntity
    {
        public string Name { get; set; }
        [ForeignKey("AssigneeId")]
        public virtual User Assignee { get; set; }
        public string AssigneeId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        public int ProjectId { get; set; }
        public Status Status { get; set; }
        public virtual List<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    }
}
