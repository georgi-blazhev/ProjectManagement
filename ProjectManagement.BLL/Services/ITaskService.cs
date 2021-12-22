using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.BLL.Services
{
    public interface ITaskService
    {
        System.Threading.Tasks.Task Create(string name, string assigneeUserName, int projectId, Status status, string creatorId);
        System.Threading.Tasks.Task Delete(int taskId);
        System.Threading.Tasks.Task Update(int taskId, string name, Status status);
        Task<List<DAL.Entities.Task>> List(string userId);
        Task<DAL.Entities.Task> GetTaskById(int taskId);
        System.Threading.Tasks.Task Reassign(int taskId, string userName);
    }
}
