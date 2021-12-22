using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BLL.Services;
using ProjectManagement.DAL.Entities;
using ProjectManagement.Models.DTO.Requests.UserRequests;
using ProjectManagement.Models.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.WEB.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
            : base()
        {
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("/create")]
        public async System.Threading.Tasks.Task Create(CreateUser user)
        {
            await _userService.CreateUser(user.UserName, user.Password, user.FirstName, user.LastName, user.Role);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("/delete/{userName}")]
        public async System.Threading.Tasks.Task Delete(string userName)
        {
            await _userService.DeleteUserAsync(userName);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("/edit/{userName}")]
        public async System.Threading.Tasks.Task Edit(EditUser user, string userName)
        {
            await _userService.UpdateUserAsync(userName, user.NewUserName, user.Password, user.FirstName, user.LastName);
        }

        [Authorize]
        [HttpGet]
        public async Task<List<UserResponse>> AllUsers()
        {
            List<User> users = await _userService.GetAll();
            List<UserResponse> all = new List<UserResponse>();
            foreach (var user in users)
            {
                all.Add(new UserResponse()
                { 
                    UserName = user.UserName, 
                    FirstName = user.FirstName, 
                    LastName = user.LastName 
                });
            }
            return all;
        }
    }
}
