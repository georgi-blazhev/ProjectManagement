using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectManagement.DAL.Data;
using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.BLL.Services
{
    public class ProjectManagementUserManager : UserManager<User>, IUserManager
    {
        private readonly DatabaseContext _databaseContext; 

        public ProjectManagementUserManager(IUserStore<User> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger, DatabaseContext databaseContext) :
            base(store,
            optionsAccessor,
            passwordHasher,
            userValidators,
            passwordValidators,
            keyNormalizer,
            errors,
            services,
            logger)
        {
            _databaseContext = databaseContext;
        }

        public async System.Threading.Tasks.Task CreateUserAsync(User user, string password)
        {
            await CreateAsync(user, password);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await Users.ToListAsync();
        }

        public async Task<User> FindByUserNameAsync(string userName)
        {
            return await FindByNameAsync(userName);
        }


        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            return await DeleteAsync(user);
        }

        public async Task<List<string>> GetUserRolesAsync(User user)
        {
            return (await GetRolesAsync(user)).ToList();
        }

        public async Task<bool> ValidateUserCredentials(string userName, string password)
        {
            User user = await FindByNameAsync(userName);
            if (user != null)
            {
                bool result = await CheckPasswordAsync(user, password);
                return result;
            }
            return false;
        }

        Task<IdentityResult> IUserManager.UpdateUserAsync(User user)
        {
            return UpdateAsync(user);
        }

        public async System.Threading.Tasks.Task DeleteUserFromTeams(string userId)
        {
            List<TeamUser> teamUsers = await _databaseContext.TeamUser.Where(tu => tu.UserId == userId).ToListAsync();

            foreach (var user in teamUsers)
            {
                _databaseContext.Remove(user);
            }

            await _databaseContext.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task HandleCascadeDeletion(string userId)
        {
            List<DAL.Entities.Task> userTasks = await _databaseContext.Tasks.Where(t => t.AssigneeId == userId).ToListAsync();
            List<Project> userProjects = await _databaseContext.Projects.Where(p => p.CreatorId == userId).ToListAsync();
            List<WorkLog> workLogs = await _databaseContext.WorkLogs.Where(w => w.CreatorId == userId).ToListAsync();

            foreach (var workLog in workLogs)
            {
                _databaseContext.Remove(workLog);
            }

            foreach (var task in userTasks)
            {
                _databaseContext.Remove(task);
            }

            foreach (var project in userProjects)
            {
                List<ProjectTeam> pt = await _databaseContext.ProjectTeam.Where(pt => pt.ProjectId == project.Id).ToListAsync();

                foreach (var p in pt)
                {
                    _databaseContext.Remove(p);
                }

                _databaseContext.Remove(project);
            }

            

            await _databaseContext.SaveChangesAsync();
        }
    }
}
