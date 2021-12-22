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
    public class TaskService : ITaskService
    {
        private readonly IUserManager _userManager;
        private readonly IRepository<DAL.Entities.Task> _taskRepository;
        private readonly IProjectService _projectService;
        private readonly ITaskManagementRepository<DAL.Entities.Task> _taskManagementRepository;
        private readonly ITeamService _teamService;

        public TaskService(IUserManager userManager, IRepository<DAL.Entities.Task> taskRepository,
            IProjectService projectService, ITaskManagementRepository<DAL.Entities.Task> taskManagementRepository,
            ITeamService teamService)
        {
            _userManager = userManager;
            _taskRepository = taskRepository;
            _projectService = projectService;
            _taskManagementRepository = taskManagementRepository;
            _teamService = teamService;
        }

        public async System.Threading.Tasks.Task Create(string name, string assigneeUserName, int projectId, Status status, string creatorId)
        {
            DAL.Entities.Task taskFromDb = _taskRepository.Get(t => t.Name == name);

            if (taskFromDb != null)
            {
                throw new AlreadyExistingException("This task already exists!");
            }

            User assignee = await _userManager.FindByUserNameAsync(assigneeUserName);

            if (assignee == null)
            {
                throw new NotExistingException("This user does not exist!");
            }

            if (!_projectService.ListMyProjects(creatorId).Result.Contains(await _projectService.GetProjectById(projectId)))
            {
                throw new UnauthorizedAccessException("You are not part of this project!");
            }

            //Checks if the assignee user's projects contain the current project id
            //i.e. checks if the assignee is either an owner of the project or is a part of a team that is assigned to this project
            if (!_projectService.ListMyProjects(assignee.Id).Result.Contains(await _projectService.GetProjectById(projectId)))
            {
                throw new NotExistingException("This user does not belong to this project");
            }

            DAL.Entities.Task task = new DAL.Entities.Task
            {
                Name = name,
                AssigneeId = assignee.Id,
                ProjectId = projectId,
                Status = status,
                CreatorId = creatorId,
                CreatedAt = DateTime.Now
            };

            await _taskRepository.Create(task);
        }

        public async System.Threading.Tasks.Task Delete(int taskId)
        {
            DAL.Entities.Task task = await _taskRepository.Get(taskId);

            if (task == null)
            {
                throw new NotExistingException("This task doesn't exist!");
            }

            await _taskRepository.Delete(task);
        }

        public async Task<DAL.Entities.Task> GetTaskById(int taskId)
        {
            DAL.Entities.Task task = await _taskRepository.Get(taskId);

            if (task == null)
            {
                throw new NotExistingException("This task doesn't exist!");
            }

            return task;
        }

        public async Task<List<DAL.Entities.Task>> List(string userId)
        {
            return await _taskManagementRepository.GetMyTasks(userId);
        }

        public async System.Threading.Tasks.Task Reassign(int taskId, string userName)
        {
            DAL.Entities.Task task = await _taskRepository.Get(taskId);
            User user = await _userManager.FindByUserNameAsync(userName);

            if (task == null)
            {
                throw new NotExistingException("This task doesn't exist!");
            }

            if (user == null)
            {
                throw new NotExistingException("This user doesn't exist!");
            }

            task.AssigneeId = user.Id;

            await _taskRepository.Update();
        }

        public async System.Threading.Tasks.Task Update(int taskId, string name, Status status)
        {
            DAL.Entities.Task task = await _taskRepository.Get(taskId);

            if (task == null)
            {
                throw new NotExistingException("This task doesn't exist!");
            }

            task.Name = name;
            task.Status = status;

            await _taskRepository.Update();
        }
    }
}
