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
    public class TeamManagementRepository<T> : ITeamManagementRepository<T> where T : AbstractEntity
    {
        private readonly DatabaseContext _databaseContext;

        public TeamManagementRepository(DatabaseContext context)
        {
            _databaseContext = context;
        }

        public async System.Threading.Tasks.Task AssignUserToTeam(string userId, int teamId)
        {
            TeamUser tu = new TeamUser() { UserId = userId, TeamId = teamId };

            await _databaseContext.AddAsync(tu);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<List<TeamUser>> GetTeamMembers(int teamId)
        {
            return await _databaseContext.TeamUser.Where(tu => tu.TeamId == teamId).ToListAsync();
        }

        public async Task<TeamUser> Get(string userId, int teamId)
        {
            return await _databaseContext.TeamUser.FirstOrDefaultAsync(tu => tu.TeamId == teamId && tu.UserId == userId);
        }

        public async System.Threading.Tasks.Task RemoveTeamMembers(int id)
        {
            List<TeamUser> teamUsers = await _databaseContext.TeamUser.Where(tu => tu.TeamId == id).ToListAsync();

            foreach (var user in teamUsers)
            {
                _databaseContext.Remove(user);
            }

            await _databaseContext.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task RemoveUserFromTeam(string userId, int teamId)
        {
            TeamUser teamUser = await _databaseContext.TeamUser.FirstOrDefaultAsync(tu => tu.TeamId == teamId && tu.UserId == userId);

            _databaseContext.Remove(teamUser);
            await _databaseContext.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task RemoveTeamFromProjects(int teamId)
        {
            List<ProjectTeam> pt = await _databaseContext.ProjectTeam.Where(pt => pt.TeamId == teamId).ToListAsync();

            foreach (var team in pt)
            {
                _databaseContext.Remove(team);
            }
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<List<TeamUser>> GetForUserIn(string userId)
        {
            return await _databaseContext.TeamUser.Where(tu => tu.UserId == userId).ToListAsync();
        }
    }
}
