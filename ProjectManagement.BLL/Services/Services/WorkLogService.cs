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
    public class WorkLogService : IWorkLogService
    {
        private readonly IUserManager _userManager;
        private readonly IRepository<WorkLog> _workLogRepository;
        private readonly ITaskService _taskService;

        public WorkLogService(IUserManager userManager, IRepository<WorkLog> workLogRepository, ITaskService taskService)
        {
            _userManager = userManager;
            _workLogRepository = workLogRepository;
            _taskService = taskService;
        }

        public async System.Threading.Tasks.Task CreateWorkLog(int timeSpent, string description, int taskWorkedOnId, string creatorId)
        {
            DAL.Entities.Task task = await _taskService.GetTaskById(taskWorkedOnId);
            User user = await _userManager.FindByIdAsync(creatorId);

            if (task == null)
            {
                throw new NotExistingException("This task does not exist!");
            }

            if (task.AssigneeId == creatorId || _userManager.GetUserRolesAsync(user).Result.Contains("Admin"))
            {
                WorkLog workLog = new WorkLog()
                {
                    TimeSpent = timeSpent,
                    Description = description,
                    TaskWorkedOnId = taskWorkedOnId,
                    CreatorId = creatorId,
                    CreatedAt = DateTime.Now
                };

                await _workLogRepository.Create(workLog);
                return;
            }

            throw new UnauthorizedAccessException("You are not authorized to post in this task!");
        }

        public async System.Threading.Tasks.Task DeleteWorkLog(int workLogId)
        {
            WorkLog workLog = await _workLogRepository.Get(workLogId);

            if (workLog == null)
            {
                throw new NotExistingException("This worklog does not exist!");
            }

            await _workLogRepository.Delete(workLog);
        }

        public async Task<WorkLog> GetWorkLog(int workLogId)
        {
            WorkLog workLog = await _workLogRepository.Get(workLogId);

            if (workLog == null)
            {
                throw new NotExistingException("This worklog does not exist!");
            }

            return workLog;
        }

        public async System.Threading.Tasks.Task UpdateWorkLog(int workLogId, int timeSpent, string description)
        {
            WorkLog workLog = await _workLogRepository.Get(workLogId);

            if (workLog == null)
            {
                throw new NotExistingException("This worklog does not exist!");
            }

            workLog.TimeSpent = timeSpent;
            workLog.Description = description;

            await _workLogRepository.Update();
        }
    }
}
