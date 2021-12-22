using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.DAL.Entities
{
    public abstract class AbstractEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatorId { get; set; }
    }
}
