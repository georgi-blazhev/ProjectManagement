using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BLL.Services;
using ProjectManagement.DAL.Entities;
using ProjectManagement.Models.DTO.Requests.WorkLogRequests;
using ProjectManagement.Models.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.WEB.Controllers
{
    [Route("api/worklogs")]
    [ApiController]
    [Authorize]
    public class WorkLogController : ControllerBase
    {
        private readonly IWorkLogService _workLogService;
        private readonly IUserService _userService;

        public WorkLogController(IWorkLogService workLogService, IUserService userService)
        {
            _workLogService = workLogService;
            _userService = userService;
        }

        [HttpPost]
        [Route("/create/worklog")]
        public async System.Threading.Tasks.Task Create(CreateWorkLog workLog)
        {
            User current = await _userService.GetCurrentUser(User);
            await _workLogService.CreateWorkLog(workLog.TimeSpent, workLog.Description, workLog.TaskWorkedOnId, current.Id);
        }

        [HttpDelete]
        [Route("/delete/worklog{workLogId}")]
        [Authorize("WorkLogAdminOrTaskAssignee")]
        public async System.Threading.Tasks.Task Delete(int workLogId)
        {
            await _workLogService.DeleteWorkLog(workLogId);
        }

        [HttpPut]
        [Route("/update/worklog{workLogId}")]
        [Authorize("WorkLogAdminOrTaskAssignee")]
        public async System.Threading.Tasks.Task Update(int workLogId, UpdateWorkLog workLog)
        {
            await _workLogService.UpdateWorkLog(workLogId, workLog.TimeSpent, workLog.Description);
        }

        [HttpGet]
        [Route("/worklog={workLogId}")]
        [Authorize("WorkLogAdminProjectCreatorOrTeamMember")]
        public async Task<WorkLogResponse> GetWorkLog(int workLogId)
        {
            WorkLog workLog = await _workLogService.GetWorkLog(workLogId);

            WorkLogResponse workLogResponse = new WorkLogResponse()
            {
                Description = workLog.Description,
                TimeSpent = workLog.TimeSpent,
                TaskWorkedOnId = workLog.TaskWorkedOnId,
                TaskWorkOnName = workLog.TaskWorkedOn.Name
            };

            return workLogResponse;
        }
    }
}
