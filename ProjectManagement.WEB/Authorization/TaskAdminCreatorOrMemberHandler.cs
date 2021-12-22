using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProjectManagement.BLL.Services;
using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.WEB.Authorization
{
    public class TaskAdminCreatorOrMemberHandler : AuthorizationHandler<TaskAdminCreatorOrMemberRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly IUserManager _userManager;
        private readonly ITaskService _taskService;

        public TaskAdminCreatorOrMemberHandler(IHttpContextAccessor httpContextAccessor,
            IUserService userService, IProjectService projectService, IUserManager userManager,
            ITaskService taskService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _projectService = projectService;
            _userManager = userManager;
            _taskService = taskService;
        }

        protected async override System.Threading.Tasks.Task HandleRequirementAsync(AuthorizationHandlerContext context,
            TaskAdminCreatorOrMemberRequirement requirement)
        {
            string currentUserName = context.User.Identity.Name;
            User current = await _userService.GetUserByName(currentUserName);
            int taskId = int.Parse(_httpContextAccessor.HttpContext.GetRouteValue("taskId").ToString());
            DAL.Entities.Task task = await _taskService.GetTaskById(taskId);

            if (task is null)
            {
                context.Fail();
                await System.Threading.Tasks.Task.CompletedTask;
                return;
            }

            if (_userManager.GetUserRolesAsync(current).Result.Contains("Admin"))
            {
                context.Succeed(requirement);
            }

            if (current.Id == task.CreatorId)
            {
                context.Succeed(requirement);
            }

            if (_projectService.ListMyProjects(current.Id).Result.Contains(await _projectService.GetProjectById(task.ProjectId)))
            {
                context.Succeed(requirement);
            }

            await System.Threading.Tasks.Task.CompletedTask;
            return;
        }
    }
}
