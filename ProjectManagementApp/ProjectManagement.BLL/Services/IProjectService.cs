using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.BLL.Services
{
    public interface IProjectService
    {
        System.Threading.Tasks.Task CreateProjectAsync(string name, string creatorId);
        System.Threading.Tasks.Task DeleteProjectAsync(int id);
        System.Threading.Tasks.Task UpdateProjectAsync(int projectId, string projectName);
        Task<List<Project>> List();
        Task<List<Project>> ListMyProjects(string userId);
        Task<Project> GetProjectById(int projectId);
        System.Threading.Tasks.Task AssignTeam(int projectId, int teamId);
    }
}
