using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.BLL.Services
{
    public interface IWorkLogService
    {
        System.Threading.Tasks.Task CreateWorkLog(int timeSpent, string description, int taskWorkedOnId, string creatorId);
        System.Threading.Tasks.Task DeleteWorkLog(int workLogId);
        System.Threading.Tasks.Task UpdateWorkLog(int workLogId, int timeSpent, string description);
        Task<WorkLog> GetWorkLog(int workLogId);

    }
}
