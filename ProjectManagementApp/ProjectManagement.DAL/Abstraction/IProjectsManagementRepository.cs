using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.DAL.Abstraction
{
    public interface IProjectsManagementRepository<T> where T : AbstractEntity
    {
        System.Threading.Tasks.Task AssignTeamToProject(int teamId, int projectId);
        Task<ProjectTeam> GetProjectTeam(int teamId, int projectId);
        Task<List<Project>> GetProjectsForUser(string userId);
        System.Threading.Tasks.Task DeleteProjectsForTeams(int projectId);
    }
}
