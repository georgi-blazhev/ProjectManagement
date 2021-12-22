using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.DAL.Entities
{
    public class Project : AbstractEntity
    {
        public string Name { get; set; }
        public virtual User Creator { get; set; }
        public virtual List<Team> Teams { get; set; } = new List<Team>();
        public virtual List<Task> Tasks { get; set; } = new List<Task>();
    }
}
