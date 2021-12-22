using Microsoft.AspNetCore.Identity;
using Moq;
using ProjectManagement.BLL.Exceptions;
using ProjectManagement.BLL.Services;
using ProjectManagement.DAL.Abstraction;
using ProjectManagement.DAL.Entities;
using ProjectManagement.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ProjectManagement.BLL.UnitTests.Services
{
    public class TeamServiceTests
    {
        [Fact]
        public async System.Threading.Tasks.Task Create_Team_Success_When_Team_Does_Not_Exist()
        {
            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var userManagerMock = new Mock<IUserManager>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var sut = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            teamRepositoryMock.Setup(tr => tr.Get(It.IsAny<Func<Team, bool>>())).Returns((Team)null);

            await sut.CreateTeamAsync(It.IsAny<string>(), It.IsAny<string>());

            teamRepositoryMock.Verify(tm => tm.Create(It.IsAny<Team>()), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_Team_Fails_When_Team_Exists()
        {
            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var userManagerMock = new Mock<IUserManager>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var sut = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            Team team = new Team() { Name = "fake", CreatedAt = DateTime.Now, CreatorId = "1" };

            teamRepositoryMock.Setup(tr => tr.Get(It.IsAny<Func<Team, bool>>())).Returns(team);

            await Assert.ThrowsAsync<AlreadyExistingException>(() => sut.CreateTeamAsync(It.IsAny<string>(), It.IsAny<string>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Assign_User_To_Team_Success_When_Team_And_User_Exist_And_User_Is_Not_In_Team()
        {
            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var userManagerMock = new Mock<IUserManager>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var sut = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            Team team = new Team();
            User user = new User();

            teamRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync(team);
            userManagerMock.Setup(um => um.FindByUserNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            teamManagementRepositoryMock.Setup(tmr => tmr.Get(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((TeamUser)null);

            await sut.AssignUserToTeam(It.IsAny<string>(), It.IsAny<int>());

            teamManagementRepositoryMock.Verify(tmr => tmr.AssignUserToTeam(It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Assign_User_To_Team_Fails_When_Team_And_User_Exist_And_User_Is_In_Team()
        {
            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var userManagerMock = new Mock<IUserManager>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var sut = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            TeamUser teamUser = new TeamUser();
            Team team = new Team();
            User user = new User();

            teamRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync(team);
            userManagerMock.Setup(um => um.FindByUserNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            teamManagementRepositoryMock.Setup(tmr => tmr.Get(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(teamUser);

            await Assert.ThrowsAsync<AlreadyExistingException>(() => sut.AssignUserToTeam(It.IsAny<string>(), It.IsAny<int>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Assign_User_To_Team_Fails_When_Team_Does_Not_Exist()
        {
            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var userManagerMock = new Mock<IUserManager>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var sut = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            TeamUser teamUser = new TeamUser();
            Team team = new Team();
            User user = new User();

            teamRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync((Team)null);
            userManagerMock.Setup(um => um.FindByUserNameAsync(It.IsAny<string>())).ReturnsAsync(user);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.AssignUserToTeam(It.IsAny<string>(), It.IsAny<int>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Assign_User_To_Team_Fails_When_User_Does_Not_Exist()
        {
            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var userManagerMock = new Mock<IUserManager>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var sut = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            TeamUser teamUser = new TeamUser();
            Team team = new Team();
            User user = new User();

            userManagerMock.Setup(um => um.FindByUserNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.AssignUserToTeam(It.IsAny<string>(), It.IsAny<int>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Delete_Team_Success_When_Team_Exists()
        {
            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var userManagerMock = new Mock<IUserManager>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var sut = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            Team team = new Team() { Name = "fake", CreatedAt = DateTime.Now, CreatorId = "1" };

            teamRepositoryMock.Setup(tp => tp.Get(It.IsAny<int>())).ReturnsAsync(team);

            await sut.DeleteTeamAsync(It.IsAny<int>());

            teamRepositoryMock.Verify(tr => tr.Delete(It.IsAny<Team>()), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Delete_Team_Fails_When_Team_Does_Not_Exist()
        {
            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var userManagerMock = new Mock<IUserManager>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var sut = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            teamRepositoryMock.Setup(tp => tp.Get(It.IsAny<int>())).ReturnsAsync((Team)null);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.DeleteTeamAsync(It.IsAny<int>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Update_Team_Success_When_Team_Exists()
        {

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var userManagerMock = new Mock<IUserManager>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var sut = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            Team team = new Team() { Name = "fake", CreatedAt = DateTime.Now, CreatorId = "1" };

            teamRepositoryMock.Setup(tp => tp.Get(It.IsAny<int>())).ReturnsAsync(team);

            await sut.UpdateTeamAsync(It.IsAny<int>(), It.IsAny<string>());

            teamRepositoryMock.Verify(tr => tr.Update(), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Update_Team_Fails_When_Team_Does_Not_Exist()
        {
            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var userManagerMock = new Mock<IUserManager>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var sut = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            teamRepositoryMock.Setup(tp => tp.Get(It.IsAny<int>())).ReturnsAsync((Team)null);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.UpdateTeamAsync(It.IsAny<int>(), It.IsAny<string>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Get_All_Teams_Returns_Repository_Collection()
        {
            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var userManagerMock = new Mock<IUserManager>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var sut = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            List<Team> teams = new List<Team>()
            {
                new Team() { Name = "Fake1" },
                new Team() { Name = "Fake2" }
            };

            teamRepositoryMock.Setup(tr => tr.All()).ReturnsAsync(teams);

            var result = await sut.GetAllTeams();

            Assert.Equal(result, teams);
        }
    }
}
