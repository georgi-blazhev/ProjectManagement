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
    public interface IUserService
    {
        System.Threading.Tasks.Task CreateUser(string userName, string password, string firstName, string lastName, string role);
        Task<IdentityResult> DeleteUserAsync(string userName);
        Task<IdentityResult> UpdateUserAsync(string userName, string newUserName, string password, string firstName, string lastName);
        Task<List<User>> GetAll();
        Task<User> GetUserById(string id);
        Task<User> GetUserByName(string name);
        Task<IdentityResult> AddToRoleAsync(User user, string role);
        Task<User> GetCurrentUser(ClaimsPrincipal principal);
    }
}
