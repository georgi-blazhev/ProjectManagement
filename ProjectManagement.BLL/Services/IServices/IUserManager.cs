using Microsoft.AspNetCore.Identity;
using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.BLL.Services
{
    public interface IUserManager
    {
        System.Threading.Tasks.Task CreateUserAsync(User user, string password);
        Task<IdentityResult> DeleteUserAsync(User user);
        Task<User> FindByUserNameAsync(string userName);
        Task<User> FindByIdAsync(string id);
        Task<IdentityResult> UpdateUserAsync(User user);
        Task<List<User>> GetAllAsync();
        Task<IdentityResult> AddToRoleAsync(User user, string role);
        Task<bool> ValidateUserCredentials(string userName, string password);
        Task<List<string>> GetUserRolesAsync(User user);
        Task<User> GetUserAsync(ClaimsPrincipal claimsPrincipal);
        System.Threading.Tasks.Task DeleteUserFromTeams(string userId);
        System.Threading.Tasks.Task HandleCascadeDeletion(string userId);
    }
}
