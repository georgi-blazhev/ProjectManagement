using Microsoft.AspNetCore.Identity;
using Moq;
using ProjectManagement.BLL.Exceptions;
using ProjectManagement.BLL.Services;
using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace ProjectManagement.BLL.UnitTests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public async System.Threading.Tasks.Task Create_User_Sucess_ValidUserName()
        {
            var userManagerMock = new Mock<IUserManager>();
            var sut = new UserService(userManagerMock.Object);

            userManagerMock.Setup(u => u.FindByUserNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            var userStub = new User { UserName = "fake-user-name", FirstName = "fake", LastName = "fake", Email = "fake@mail.fake" };

            await sut.CreateUser(userStub.UserName, "password", userStub.FirstName, userStub.LastName, "RegularUser");

            userManagerMock.Verify(um => um.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once());           
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_User_Fails_When_User_Exists()
        {
            var userManagerMock = new Mock<IUserManager>();
            var sut = new UserService(userManagerMock.Object);

            var user = new User { UserName = "fake-user-name", FirstName = "fake", LastName = "fake", Email = "fake@mail.fake" };

            userManagerMock.Setup(u => u.FindByUserNameAsync(It.IsAny<string>())).ReturnsAsync(user);

            await Assert.ThrowsAsync<AlreadyExistingException>(() => sut.CreateUser("fake-user-name", "password", "fake", "fake", "fake@mail.fake"));
        }

        [Fact]
        public async System.Threading.Tasks.Task Delete_User_Sucess_When_User_Exists()
        {
            var userManagerMock = new Mock<IUserManager>();
            var sut = new UserService(userManagerMock.Object);

            var user = new User { UserName = "fake-user-name", FirstName = "fake", LastName = "fake", Email = "fake@mail.fake" };

            userManagerMock.Setup(u => u.FindByUserNameAsync(It.IsAny<string>())).ReturnsAsync(user);

            await sut.DeleteUserAsync(user.UserName);

            userManagerMock.Verify(um => um.DeleteUserAsync(user), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Delete_User_Fails_When_User_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();
            var sut = new UserService(userManagerMock.Object);

            var user = new User { UserName = "fake-user-name", FirstName = "fake", LastName = "fake", Email = "fake@mail.fake" };

            userManagerMock.Setup(u => u.FindByUserNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.DeleteUserAsync(user.UserName));
        }

        [Fact]
        public async System.Threading.Tasks.Task Edit_User_Sucess_When_User_Exists()
        {
            var userManagerMock = new Mock<IUserManager>();
            var sut = new UserService(userManagerMock.Object);

            var user = new User { UserName = "fake-user-name", FirstName = "fake", LastName = "fake", Email = "fake@mail.fake" };

            userManagerMock.Setup(u => u.FindByUserNameAsync("fake-user-name")).ReturnsAsync(user);

            await sut.UpdateUserAsync("fake-user-name", "fake-edited-name", "fake", "fake", "fake@mail.com");

            userManagerMock.Verify(um => um.UpdateUserAsync(user), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Edit_User_Fails_When_User_DoesNotExist()
        {
            var userManagerMock = new Mock<IUserManager>();
            var sut = new UserService(userManagerMock.Object);

            userManagerMock.Setup(u => u.FindByUserNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.UpdateUserAsync("fake-user-name", "fake-edited-name", "fake", "fake", "fake@mail.com"));
        }

        [Fact]
        public async System.Threading.Tasks.Task Edit_User_Fails_When_UserNames_Overlap()
        {
            var userManagerMock = new Mock<IUserManager>();
            var sut = new UserService(userManagerMock.Object);

            var user = new User { UserName = "fake-user-name", FirstName = "fake", LastName = "fake", Email = "fake@mail.fake" };
            userManagerMock.Setup(u => u.FindByUserNameAsync(It.IsAny<string>())).ReturnsAsync(user);

            await Assert.ThrowsAsync<AlreadyExistingException>(() => sut.UpdateUserAsync("fake-user-name", "fake-user-name", "fake", "fake", "fake@mail.com"));
        }

        [Fact]
        public async System.Threading.Tasks.Task Get_All_Returns_Repository_Collection()
        {
            var users = new List<User>
            {
                new User { UserName = "user-1" },
                new User { UserName = "user-2" },
            };
            var userManagerMock = new Mock<IUserManager>();
            userManagerMock.Setup(u => u.GetAllAsync()).ReturnsAsync(users);
            var sut = new UserService(userManagerMock.Object);

            var result = await sut.GetAll();

            Assert.Equal(users, result);
        }

    }
}
