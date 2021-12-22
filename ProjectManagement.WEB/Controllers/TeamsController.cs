using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BLL.Services;
using ProjectManagement.DAL.Entities;
using ProjectManagement.Models.DTO.Requests.TeamRequests;
using ProjectManagement.Models.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.WEB.Controllers
{
    [Route("api/teams")]   
    [ApiController]
    [Authorize(Roles = "Admin, Manager")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IUserService _userService;

        public TeamsController(ITeamService teamService, IUserService userService)
        {
            _teamService = teamService;
            _userService = userService;
        }

        [HttpPost]
        public async System.Threading.Tasks.Task Create(CreateTeam team)
        {
            User current = await _userService.GetCurrentUser(User);
            await _teamService.CreateTeamAsync(team.Name, current.Id);
        }

        [HttpPost]
        [Route("/addMember/team={teamId}/user={userName}")]
        public async System.Threading.Tasks.Task AddTeamMember(string userName, int teamId)
        {
            await _teamService.AssignUserToTeam(userName, teamId);
        }

        [HttpPut]
        [Route("/edit/team={teamId}")]
        public async System.Threading.Tasks.Task EditTeam(int teamId, EditTeam team)
        {
            await _teamService.UpdateTeamAsync(teamId, team.Name);
        }

        [HttpDelete]
        public async System.Threading.Tasks.Task Delete(int id)
        {
            await _teamService.DeleteTeamAsync(id);
        }

        [HttpDelete]
        [Route("/delete/user={userName}/team={teamId}")]
        public async System.Threading.Tasks.Task RemoveTeamMember(string userName, int teamId)
        {
            await _teamService.DeleteUserFromTeam(userName, teamId);
        }

        [HttpGet]
        public async Task<List<TeamsResponse>> GetTeams()
        {
            List<TeamsResponse> allTeams = new List<TeamsResponse>();
            List<Team> teams = await _teamService.GetAllTeams();

            foreach (var team in teams)
            {
                List <User> members = await _teamService.GetTeamMembers(team.Id);
                List<UserResponse> membersToReturn = new List<UserResponse>();

                foreach (var member in members)
                {
                    membersToReturn.Add(new UserResponse()
                    {
                        UserName = member.UserName,
                        FirstName = member.FirstName,
                        LastName = member.LastName
                    });
                }

                allTeams.Add(new TeamsResponse()
                {
                    Id = team.Id,
                    Name = team.Name,
                    Members = membersToReturn
                });
            }
            return allTeams;
        }
    }
}
