using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.DAL.Entities
{
    public class WorkLog : AbstractEntity
    {
        [ForeignKey("CreatorId")]
        public virtual User User { get; set; }
        public int TimeSpent { get; set; }
        public string Description { get; set; }
        [ForeignKey("TaskWorkedOnId")]
        public virtual DAL.Entities.Task TaskWorkedOn { get; set; }
        public int TaskWorkedOnId { get; set; }
    }
}
