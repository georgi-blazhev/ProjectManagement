using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.DAL.Entities
{
    public class Team : AbstractEntity
    {
        public string Name { get; set; }
        public virtual List<User> Users { get; set; } = new List<User>();
        public virtual List<Project> Projects { get; set; } = new List<Project>();
    }
}
