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
    public class WorkLogAdminOrTaskAssigneeHandler : AuthorizationHandler<WorkLogAdminOrTaskAssigneeRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IUserManager _userManager;
        private readonly ITaskService _taskService;
        private readonly IWorkLogService _workLogService;

        public WorkLogAdminOrTaskAssigneeHandler(IHttpContextAccessor httpContextAccessor,
            IUserService userService, IUserManager userManager, ITaskService taskService, IWorkLogService workLogService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _userManager = userManager;
            _taskService = taskService;
            _workLogService = workLogService;
        }

        protected async override System.Threading.Tasks.Task HandleRequirementAsync(AuthorizationHandlerContext context, WorkLogAdminOrTaskAssigneeRequirement requirement)
        {
            string currentUserName = context.User.Identity.Name;
            User current = await _userService.GetUserByName(currentUserName);

            int workLogId = int.Parse(_httpContextAccessor.HttpContext.GetRouteValue("workLogId").ToString());
            WorkLog workLog = await _workLogService.GetWorkLog(workLogId);

            if (workLog == null)
            {
                context.Fail();
                await System.Threading.Tasks.Task.CompletedTask;
                return;
            }

            DAL.Entities.Task task = workLog.TaskWorkedOn;

            if (_userManager.GetUserRolesAsync(current).Result.Contains("Admin"))
            {
                context.Succeed(requirement);
            }

            if (current.Id == task.AssigneeId)
            {
                context.Succeed(requirement);
            }

            await System.Threading.Tasks.Task.CompletedTask;
            return;
        }
    }
}
