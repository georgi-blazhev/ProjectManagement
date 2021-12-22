using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.BLL.Services
{
    public interface ITeamService
    {
        System.Threading.Tasks.Task CreateTeamAsync(string name, string creatorId);
        System.Threading.Tasks.Task DeleteTeamAsync(int id);
        System.Threading.Tasks.Task AssignUserToTeam(string userName, int teamId);
        System.Threading.Tasks.Task UpdateTeamAsync(int id, string name);
        System.Threading.Tasks.Task DeleteUserFromTeam(string userName, int teamId);
        Task<List<User>> GetTeamMembers(int teamId);
        Task<List<Team>> GetAllTeams();
        Task<bool> AreTeamMembers(string firstUserId, string secondUserId);
    }
}
