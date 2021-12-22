using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ProjectManagement.BLL.Exceptions;
using ProjectManagement.BLL.Services;
using ProjectManagement.DAL.Entities;

namespace ProjectManagement.WEB.Authorization
{
    public class ProjectAdminOrCreatorHandler : AuthorizationHandler<ProjectAdminOrCreatorRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly IUserManager _userManager;

        public ProjectAdminOrCreatorHandler(IHttpContextAccessor httpContextAccessor, IUserService userService, IProjectService projectService,
            IUserManager userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _projectService = projectService;
            _userManager = userManager;
        }

        protected async override System.Threading.Tasks.Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAdminOrCreatorRequirement requirement)
        {
            string currentUserName = context.User.Identity.Name;
            User current = await _userService.GetUserByName(currentUserName);

            int projectId = int.Parse(_httpContextAccessor.HttpContext.GetRouteValue("projectId").ToString());

            Project project = await _projectService.GetProjectById(projectId);

            if (project == null)
            {
                context.Fail();
                await System.Threading.Tasks.Task.CompletedTask;
                return;
            }

            if (project.CreatorId == current.Id)
            {
                context.Succeed(requirement);
            }

            if (_userManager.GetUserRolesAsync(current).Result.Contains("Admin"))
            {
                context.Succeed(requirement);
            }

            await System.Threading.Tasks.Task.CompletedTask;
            return;
        }
    }
}
