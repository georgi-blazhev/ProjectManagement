using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Models.DTO.Requests.TeamRequests
{
    public class EditTeam
    {
        [Required]
        public string Name { get; set; }
    }
}
