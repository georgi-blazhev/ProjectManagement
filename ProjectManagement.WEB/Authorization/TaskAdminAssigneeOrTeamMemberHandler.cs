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
    public class TaskAdminAssigneeOrTeamMemberHandler : AuthorizationHandler<TaskAdminAssigneeOrTeamMemberRequirement>
    {
        private readonly IUserManager _userManager;
        private readonly ITaskService _taskService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;

        public TaskAdminAssigneeOrTeamMemberHandler(IUserManager userManager, ITaskService taskService,
            IHttpContextAccessor httpContextAccessor, IUserService userService, ITeamService teamService)
        {
            _userManager = userManager;
            _taskService = taskService;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _teamService = teamService;
        }

        protected async override System.Threading.Tasks.Task HandleRequirementAsync(AuthorizationHandlerContext context, TaskAdminAssigneeOrTeamMemberRequirement requirement)
        {
            string currentUserName = context.User.Identity.Name;
            User current = await _userService.GetUserByName(currentUserName);
            int taskId = int.Parse(_httpContextAccessor.HttpContext.GetRouteValue("taskId").ToString());

            string newAssigneeUserName = _httpContextAccessor.HttpContext.GetRouteValue("newAssigneeUserName").ToString();

            User newAssignee = await _userService.GetUserByName(newAssigneeUserName);

            DAL.Entities.Task task = await _taskService.GetTaskById(taskId);

            if (task is null)
            {
                context.Fail();
                await System.Threading.Tasks.Task.CompletedTask;
                return;
            }

            if (newAssignee is null)
            {
                context.Fail();
                await System.Threading.Tasks.Task.CompletedTask;
                return;
            }

            if (_userManager.GetUserRolesAsync(current).Result.Contains("Admin"))
            {
                context.Succeed(requirement);
            }

            if (current.Id == task.AssigneeId && await _teamService.AreTeamMembers(current.Id, newAssignee.Id))
            {
                context.Succeed(requirement);
            }

            await System.Threading.Tasks.Task.CompletedTask;
            return;
        }
    }
}
