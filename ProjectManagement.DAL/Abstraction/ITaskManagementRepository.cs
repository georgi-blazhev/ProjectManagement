using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.DAL.Abstraction
{
    public interface ITaskManagementRepository<T> where T : AbstractEntity
    {
        Task<List<DAL.Entities.Task>> GetMyTasks(string userId);
    }
}
