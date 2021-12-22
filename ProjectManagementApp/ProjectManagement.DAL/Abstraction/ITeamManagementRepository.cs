using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.DAL.Abstraction
{
    public interface ITeamManagementRepository<T> where T : AbstractEntity
    {
        System.Threading.Tasks.Task AssignUserToTeam(string userId, int teamId);
        Task<TeamUser> Get(string userId, int teamId);
        Task<List<TeamUser>> GetTeamMembers(int teamId);
        System.Threading.Tasks.Task RemoveUserFromTeam(string userId, int teamId);
        System.Threading.Tasks.Task RemoveTeamMembers(int id);
        System.Threading.Tasks.Task RemoveTeamFromProjects(int teamId);
        Task<List<TeamUser>> GetForUserIn(string userId);
    }
}
