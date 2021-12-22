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
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IUserManager _userManager;
        private readonly IRepository<Team> _teamRepository;
        private readonly IProjectsManagementRepository<Project> _projectsManagementRepository;

        public ProjectService(IRepository<Project> projectRepository, IUserManager userManager,
            IProjectsManagementRepository<Project> projectsManagementRepository, IRepository<Team> teamRepository)
        {
            _projectRepository = projectRepository;
            _userManager = userManager;
            _projectsManagementRepository = projectsManagementRepository;
            _teamRepository = teamRepository;
        }

        public async System.Threading.Tasks.Task AssignTeam(int projectId, int teamId)
        {
            if (await _projectRepository.Get(projectId) == null || await _teamRepository.Get(teamId) == null)
            {
                throw new NotExistingException("Project or team do not exist!");
            }

            if (await _projectsManagementRepository.GetProjectTeam(teamId, projectId) != null) 
            {
                throw new AlreadyExistingException("This team is already assigned to this project!");
            }

            await _projectsManagementRepository.AssignTeamToProject(teamId, projectId);
        }

        public async System.Threading.Tasks.Task CreateProjectAsync(string name, string creatorId)
        {
            Project projectFromDb = _projectRepository.Get(p => p.Name == name);

            if (projectFromDb != null)
            {
                throw new AlreadyExistingException("This project already exists!");
            }

            Project project = new Project() { Name = name, CreatedAt = DateTime.Now, CreatorId = creatorId };

            await _projectRepository.Create(project);
        }

        public async System.Threading.Tasks.Task DeleteProjectAsync(int id)
        {
            Project projectFromDb = await _projectRepository.Get(id);

            if (projectFromDb == null)
            {
                throw new NotExistingException("This project does not exist!");
            }

            await _projectsManagementRepository.DeleteProjectsForTeams(id);
            await _projectRepository.Delete(projectFromDb);
        }

        public async Task<Project> GetProjectById(int projectId)
        {
            Project project =  await _projectRepository.Get(projectId);

            if (project == null)
            {
                throw new NotExistingException("This project doesn't exist!");
            }

            return project;
        }

        public async Task<List<Project>> List()
        {
            return await _projectRepository.All();
        }

        public async Task<List<Project>> ListMyProjects(string userId)
        {
            return await _projectsManagementRepository.GetProjectsForUser(userId);
        }

        public async System.Threading.Tasks.Task UpdateProjectAsync(int projectId, string projectName)
        {
            Project project = await _projectRepository.Get(projectId);

            if (project == null)
            {
                throw new NotExistingException("This project doesn't exist!");
            }

            project.Name = projectName;

            await _projectRepository.Update();
        }
    }
}
