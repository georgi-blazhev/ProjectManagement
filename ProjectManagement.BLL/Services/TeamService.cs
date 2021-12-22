using ProjectManagement.BLL.Exceptions;
using ProjectManagement.DAL.Abstraction;
using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.BLL.Services
{
    public class TeamService : ITeamService
    {
        private readonly IRepository<Team> _teamRepository;
        private readonly IUserManager _userManager;
        private readonly ITeamManagementRepository<Team> _teamManagementRepostitory;

        public TeamService(IRepository<Team> teamRepository, IUserManager userManager, ITeamManagementRepository<Team> teamManagementRepostitory)
        {
            _teamRepository = teamRepository;
            _userManager = userManager;
            _teamManagementRepostitory = teamManagementRepostitory;
        }

        public async System.Threading.Tasks.Task AssignUserToTeam(string userName, int teamId)
        {
            User user = await _userManager.FindByUserNameAsync(userName);
            Team team = await _teamRepository.Get(teamId);

            if (user == null || team == null)
            {
                throw new NotExistingException("User or team do not exist!");
            }

            if (await _teamManagementRepostitory.Get(user.Id, teamId) != null)
            {
                throw new AlreadyExistingException("User is already in this team!");
            }

            await _teamManagementRepostitory.AssignUserToTeam(user.Id, teamId);
        }

        public async System.Threading.Tasks.Task CreateTeamAsync(string name, string creatorId)
        {
            Team teamFromDb = _teamRepository.Get(teamFromDb => teamFromDb.Name == name);

            if (teamFromDb != null)
            {
                throw new AlreadyExistingException("This team already exists!");
            }

            Team team = new Team() { Name = name, CreatedAt = DateTime.Now, CreatorId = creatorId };

            await _teamRepository.Create(team);
        }

        public async System.Threading.Tasks.Task DeleteTeamAsync(int id)
        {
            Team teamFromDb = await _teamRepository.Get(id);

            if (teamFromDb == null)
            {
                throw new NotExistingException("This team does not exist!");
            }

            await _teamManagementRepostitory.RemoveTeamMembers(id);
            await _teamManagementRepostitory.RemoveTeamFromProjects(id);
            await _teamRepository.Delete(teamFromDb);
        }

        public async Task<List<Team>> GetAllTeams()
        {
            return await _teamRepository.All();
        }

        public async Task<List<User>> GetTeamMembers(int teamId)
        {
            List<User> users = new List<User>();
            List<TeamUser> teamUsers = await _teamManagementRepostitory.GetTeamMembers(teamId);

            foreach (var tu in teamUsers)
            {
                users.Add(await _userManager.FindByIdAsync(tu.UserId));
            }
            return users;
        }

        public async System.Threading.Tasks.Task UpdateTeamAsync(int id, string name)
        {
            //TODO : CheckForNull
            Team team = await _teamRepository.Get(id);

            if (team == null)
            {
                throw new NotExistingException("This team does not exist!");
            }

            team.Name = name;
            await _teamRepository.Update();
        }

        public async System.Threading.Tasks.Task DeleteUserFromTeam(string userName, int teamId)
        {
            //TODO : CheckForNull
            User user = await _userManager.FindByUserNameAsync(userName);
            Team team = await _teamRepository.Get(teamId);

            if (user == null || team == null)
            {
                throw new NotExistingException("User or team do not exist!");
            }

            await _teamManagementRepostitory.RemoveUserFromTeam(user.Id, teamId);
        }

        public async Task<bool> AreTeamMembers(string firstUserId, string secondUserId)
        {
            List<TeamUser> firstUserTeams = await _teamManagementRepostitory.GetForUserIn(firstUserId);
            List<TeamUser> secondUserTeams = await _teamManagementRepostitory.GetForUserIn(secondUserId);

            List<int> firstUserTeamIds = new List<int>();
            List<int> secondUserTeamIds = new List<int>();

            foreach (var team in firstUserTeams)
            {
                firstUserTeamIds.Add(team.TeamId);
            }

            foreach (var team in secondUserTeams)
            {
                secondUserTeamIds.Add(team.TeamId);
            }

            firstUserTeamIds.Distinct();
            secondUserTeamIds.Distinct();

            for (int i = 0; i < firstUserTeamIds.Count; i++)
            {
                for (int j = 0; j < secondUserTeamIds.Count; j++)
                {
                    if (firstUserTeamIds[i] == secondUserTeamIds[j])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
