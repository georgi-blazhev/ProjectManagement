using Microsoft.EntityFrameworkCore;
using ProjectManagement.DAL.Abstraction;
using ProjectManagement.DAL.Data;
using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.DAL.Repositories
{
    public class ProjectsManagementRepository<T> : IProjectsManagementRepository<T> where T : AbstractEntity
    {
        private readonly DatabaseContext _database;

        public ProjectsManagementRepository(DatabaseContext database)
        {
            _database = database;
        }

        public async System.Threading.Tasks.Task AssignTeamToProject(int teamId, int projectId)
        {
            ProjectTeam pt = new ProjectTeam() { ProjectId = projectId, TeamId = teamId };

            await _database.AddAsync(pt);
            await _database.SaveChangesAsync();
        }

        public async Task<ProjectTeam> GetProjectTeam(int teamId, int projectId)
        {
            return await _database.ProjectTeam.FirstOrDefaultAsync(pt => pt.ProjectId == projectId && pt.TeamId == teamId);
        }

        public async Task<List<Project>> GetProjectsForUser(string userId)
        {
            var projects = from p in _database.Projects
                           join pt in _database.ProjectTeam on p.Id equals pt.ProjectId
                           join tu in _database.TeamUser on pt.TeamId equals tu.TeamId
                           where tu.UserId == userId
                           select p;

            var created = from p in _database.Projects
                          where p.CreatorId == userId
                          select p;

            var result = created.Union(projects).OrderBy(r => r.Id);

            return await result.ToListAsync();
        }

        public async System.Threading.Tasks.Task DeleteProjectsForTeams(int projectId)
        {
            List<ProjectTeam> pt = await _database.ProjectTeam.Where(pt => pt.ProjectId == projectId).ToListAsync();

            foreach (var project in pt)
            {
                _database.Remove(project);            
            }
            await _database.SaveChangesAsync();
        }
    }
}
