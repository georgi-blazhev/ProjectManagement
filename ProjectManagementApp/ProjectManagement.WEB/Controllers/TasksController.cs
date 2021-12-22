using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BLL.Services;
using ProjectManagement.DAL.Entities;
using ProjectManagement.Models.DTO.Requests.TaskRequests;
using ProjectManagement.Models.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.WEB.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITaskService _tasksService;

        public TasksController(IUserService userService, ITaskService tasksService)
        {
            _userService = userService;
            _tasksService = tasksService;
        }

        [HttpPost]
        [Route("/create/task")]
        public async System.Threading.Tasks.Task Create(CreateTask task)
        {
            User current = await _userService.GetCurrentUser(User);

            await _tasksService.Create(task.Name, task.AssigneeUserName, task.ProjectId, DAL.Entities.Status.Pending, current.Id);
        }

        [HttpDelete]
        [Authorize("TaskAdminCreatorOrMember")]
        [Route("/delete/task={taskId}")]
        public async System.Threading.Tasks.Task Delete(int taskId)
        {
            await _tasksService.Delete(taskId);
        }

        [HttpPut]
        [Authorize("TaskAdminCreatorOrMember")]
        [Route("/update/task={taskId}")]
        public async System.Threading.Tasks.Task Update(int taskId, EditTask task)
        {
            await _tasksService.Update(taskId, task.Name, (Status)Enum.Parse(typeof(Status), task.Status, true));
        }


        [HttpPost]
        [Authorize("TaskAdminAssigneeOrTeamMember")]
        [Route("/reassign/task={taskId}/user={newAssigneeUserName}")]
        public async System.Threading.Tasks.Task Reassign(int taskId, string newAssigneeUserName)
        {
            await _tasksService.Reassign(taskId, newAssigneeUserName);
        }
        
        [HttpGet]
        [Route("/task/{taskId}")]
        public async Task<TasksResponse> GetById(int taskId)
        {
            DAL.Entities.Task task = await _tasksService.GetTaskById(taskId);

            TasksResponse taskResponse = new TasksResponse()
            {
                Name = task.Name,
                ProjectId = task.ProjectId,
                ProjectName = task.Project.Name,
                AssigneeName = task.Assignee.UserName,
                Status = task.Status.ToString()
            };

            return taskResponse;
        }

        [HttpGet]
        [Route("mytasks/")]
        public async Task<List<TasksResponse>> MyTasks()
        {
            User current = await _userService.GetCurrentUser(User);
            List<DAL.Entities.Task> tasks = await _tasksService.List(current.Id);
            List<TasksResponse> tasksResponses = new List<TasksResponse>();

            foreach (var task in tasks)
            {
                tasksResponses.Add(new TasksResponse()
                {
                    Name = task.Name,
                    ProjectId = task.ProjectId,
                    ProjectName = task.Project.Name,
                    AssigneeName = task.Assignee.UserName,
                    Status = task.Status.ToString()
                });
            }
            return tasksResponses;
        }
    }
}
