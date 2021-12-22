using Microsoft.EntityFrameworkCore;
using ProjectManagement.DAL.Abstraction;
using ProjectManagement.DAL.Data;
using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.DAL.Repositories
{
    public class TaskManagementRepository<T> : ITaskManagementRepository<T> where T : AbstractEntity
    {
        private readonly DatabaseContext _database;

        public TaskManagementRepository(DatabaseContext database)
        {
            _database = database;
        }

        public async Task<List<Entities.Task>> GetMyTasks(string userId)
        {
            var query = from t in _database.Tasks
                        join pt in _database.ProjectTeam on t.ProjectId equals pt.ProjectId
                        join tu in _database.TeamUser on pt.TeamId equals tu.TeamId
                        where tu.UserId == userId || t.CreatorId == userId || t.AssigneeId == userId
                        select t;

            return await query.Distinct().ToListAsync();
        }
    }
}
