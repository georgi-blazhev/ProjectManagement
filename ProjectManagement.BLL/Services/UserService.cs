using Microsoft.AspNetCore.Identity;
using ProjectManagement.BLL.Exceptions;
using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserManager _userManager;

        public UserService(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async System.Threading.Tasks.Task CreateUser(string userName, string password, string firstName, string lastName, string role)
        {
            if (await _userManager.FindByUserNameAsync(userName) != null)
            {
                throw new AlreadyExistingException("This user already exists!");
            }

            User user = new User() { UserName = userName, FirstName = firstName, LastName = lastName };

            await _userManager.CreateUserAsync(user, password);

            try
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            catch (System.InvalidOperationException)
            {
                await DeleteUserAsync(userName);
                throw new NotExistingException($"Role {role} doesn't exist!");
            }

        }

        public async Task<IdentityResult> DeleteUserAsync(string userName)
        {
            var user = await _userManager.FindByUserNameAsync(userName);

            if (user == null)
            {
                throw new NotExistingException("This user doesn't exist!");
            }

            await _userManager.HandleCascadeDeletion(user.Id);
            await _userManager.DeleteUserFromTeams(user.Id);

            return await _userManager.DeleteUserAsync(user);
        }

        public async Task<List<User>> GetAll()
        {
            return await _userManager.GetAllAsync();
        }

        public async Task<User> GetCurrentUser(ClaimsPrincipal principal)
        {
            return await _userManager.GetUserAsync(principal);
        }

        public async Task<User> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<User> GetUserByName(string userName)
        {
            return await _userManager.FindByUserNameAsync(userName);
        }

        public async Task<IdentityResult> UpdateUserAsync(string userName, string newUserName, string password, string firstName, string lastName)
        {
            User user = await _userManager.FindByUserNameAsync(userName);

            if (user == null)
            {
                throw new NotExistingException("This user doesn't exist!");
            }

            User newUserNameUser = await _userManager.FindByUserNameAsync(newUserName);

            if (newUserNameUser != null)
            {
                throw new AlreadyExistingException("This user already exists!");
            }

            PasswordHasher<User> hasher = new PasswordHasher<User>();

            user.UserName = newUserName;
            user.PasswordHash = hasher.HashPassword(user, password);
            user.FirstName = firstName;
            user.LastName= lastName;

            return await _userManager.UpdateUserAsync(user);
        }
    }
}
