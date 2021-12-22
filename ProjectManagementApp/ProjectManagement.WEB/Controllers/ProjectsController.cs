using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BLL.Services;
using ProjectManagement.DAL.Entities;
using ProjectManagement.Models.DTO.Requests.ProjectRequests;
using ProjectManagement.Models.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.WEB.Controllers
{
    [Route("api/projects")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;

        public ProjectsController(IProjectService projectService, IUserService userService)
        {
            _projectService = projectService;
            _userService = userService;
        }

        [HttpPost]
        [Route("/projects/create")]
        public async System.Threading.Tasks.Task Create(CreateProject project)
        {
            User user = await _userService.GetCurrentUser(User);
            await _projectService.CreateProjectAsync(project.Name, user.Id);
        }

        [HttpPost]
        [Authorize("ProjectAdminOrCreator")]
        [Route("/assign/project={projectId}/team={teamId}")]
        public async System.Threading.Tasks.Task Assign(int projectId, int teamId)
        {
            await _projectService.AssignTeam(projectId, teamId);
        }

        [HttpDelete]
        [Route("/delete/project={projectId}")]
        [Authorize("ProjectAdminOrCreator")]
        public async System.Threading.Tasks.Task Delete(int projectId)
        {
            await _projectService.DeleteProjectAsync(projectId);
        }

        [HttpPut]
        [Route("/update/project={projectId}")]
        [Authorize("ProjectAdminOrCreator")]
        public async System.Threading.Tasks.Task Update(int projectId, EditProject project)
        {
            await _projectService.UpdateProjectAsync(projectId, project.Name);
        }

        [HttpGet]
        [Route("/all")]
        public async Task<List<ProjectsResponse>> List()
        {
            List<Project> projects = await _projectService.List();
            List<ProjectsResponse> responses = new List<ProjectsResponse>();

            foreach (var project in projects)
            {
                responses.Add(new ProjectsResponse()
                {
                    Id = project.Id,
                    Name = project.Name
                });
            }
            return responses;
        }

        [HttpGet]
        [Route("/myprojects")]
        public async Task<List<ProjectsResponse>> ListMy()
        {
            User user = await _userService.GetCurrentUser(User);

            List<Project> projects = await _projectService.ListMyProjects(user.Id);
            List<ProjectsResponse> responses = new List<ProjectsResponse>();

            foreach (var project in projects)
            {
                responses.Add(new ProjectsResponse()
                {
                    Id = project.Id,
                    Name = project.Name
                });
            }
            return responses;
        }
    }
}
