using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Models.DTO.Responses
{
    public class TeamsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UserResponse> Members { get; set; } = new List<UserResponse>();
    }
}
