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
    public class TaskServiceTests
    {
        [Fact]
        public async System.Threading.Tasks.Task Create_Task_Success_When_Valid()
        {
            var userManagerMock = new Mock<IUserManager>();          

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementMock = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var sut = new TaskService(userManagerMock.Object, taskRepositoryMock.Object, projectServiceStub, taskManagementMock.Object, teamServiceStub);

            taskRepositoryMock.Setup(tr => tr.Get(It.IsAny<Func<DAL.Entities.Task, bool>>())).Returns((DAL.Entities.Task)null);
            userManagerMock.Setup(um => um.FindByUserNameAsync(It.IsAny<string>())).ReturnsAsync(new User());

            Project project = new Project();

            List<Project> projects = new List<Project>();

            projects.Add(project);

            projectManagementRepositoryMock.Setup(pmr => pmr.GetProjectsForUser(It.IsAny<string>())).ReturnsAsync(projects);
            projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<int>())).ReturnsAsync(project);

            await sut.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Status>(), It.IsAny<string>());

            taskRepositoryMock.Verify(tr => tr.Create(It.IsAny<DAL.Entities.Task>()), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_Task_Fails_When_Task_Exists()
        {
            var userManagerMock = new Mock<IUserManager>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementMock = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var sut = new TaskService(userManagerMock.Object, taskRepositoryMock.Object, projectServiceStub, taskManagementMock.Object, teamServiceStub);

            taskRepositoryMock.Setup(tr => tr.Get(It.IsAny<Func<DAL.Entities.Task, bool>>())).Returns(new DAL.Entities.Task());

            await Assert.ThrowsAsync<AlreadyExistingException>(() =>
            sut.Create(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<Status>(),
                It.IsAny<string>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_Task_Fails_When_Task_Assignee_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementMock = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var sut = new TaskService(userManagerMock.Object, taskRepositoryMock.Object, projectServiceStub, taskManagementMock.Object, teamServiceStub);

            taskRepositoryMock.Setup(tr => tr.Get(It.IsAny<Func<DAL.Entities.Task, bool>>())).Returns((DAL.Entities.Task)null);
            userManagerMock.Setup(um => um.FindByUserNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NotExistingException>(() =>
            sut.Create(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<Status>(),
                It.IsAny<string>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_Task_Fails_When_Task_Assignee_Creator_Is_Not_Part_Of_Project()
        {
            var userManagerMock = new Mock<IUserManager>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementMock = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var sut = new TaskService(userManagerMock.Object, taskRepositoryMock.Object, projectServiceStub, taskManagementMock.Object, teamServiceStub);

            taskRepositoryMock.Setup(tr => tr.Get(It.IsAny<Func<DAL.Entities.Task, bool>>())).Returns((DAL.Entities.Task)null);
            userManagerMock.Setup(um => um.FindByUserNameAsync(It.IsAny<string>())).ReturnsAsync(new User());

            Project project = new Project();

            List<Project> projects = new List<Project>();

            projectManagementRepositoryMock.Setup(pmr => pmr.GetProjectsForUser(It.IsAny<string>())).ReturnsAsync(projects);
            projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<int>())).ReturnsAsync(project);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            sut.Create(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<Status>(),
                It.IsAny<string>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_Task_Fails_When_Project_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementMock = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var sut = new TaskService(userManagerMock.Object, taskRepositoryMock.Object, projectServiceStub, taskManagementMock.Object, teamServiceStub);

            taskRepositoryMock.Setup(tr => tr.Get(It.IsAny<Func<DAL.Entities.Task, bool>>())).Returns((DAL.Entities.Task)null);
            userManagerMock.Setup(um => um.FindByUserNameAsync(It.IsAny<string>())).ReturnsAsync(new User());

            List<Project> projects = new List<Project>();

            projectManagementRepositoryMock.Setup(pmr => pmr.GetProjectsForUser(It.IsAny<string>())).ReturnsAsync(projects);
            projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<int>())).ReturnsAsync((Project)null);

            await Assert.ThrowsAsync<NotExistingException>(() =>
            sut.Create(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<Status>(),
                It.IsAny<string>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Delete_Task_Success_When_Task_Exists()
        {
            var userManagerMock = new Mock<IUserManager>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementMock = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var sut = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementMock.Object, teamServiceStub);

            taskRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync(new DAL.Entities.Task());

            await sut.Delete(It.IsAny<int>());

            taskRepositoryMock.Verify(tr => tr.Delete(It.IsAny<DAL.Entities.Task>()), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Delete_Task_Fails_When_Task_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementMock = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var sut = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementMock.Object, teamServiceStub);

            taskRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync((DAL.Entities.Task)null);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Update_Task_Success_When_Task_Exists()
        {
            var userManagerMock = new Mock<IUserManager>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementMock = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var sut = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementMock.Object, teamServiceStub);

            taskRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync(new DAL.Entities.Task());

            await sut.Update(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Status>());

            taskRepositoryMock.Verify(tr => tr.Update(), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Update_Task_Fails_When_Task_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementMock = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var sut = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementMock.Object, teamServiceStub);

            taskRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync((DAL.Entities.Task)null);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.Update(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Status>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Get_Task_Success_When_Task_Exists()
        {
            var userManagerMock = new Mock<IUserManager>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementMock = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var sut = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementMock.Object, teamServiceStub);

            DAL.Entities.Task task = new DAL.Entities.Task();

            taskRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync(task);

            var result = await sut.GetTaskById(It.IsAny<int>());

            Assert.Equal(task, result);
        }

        [Fact]
        public async System.Threading.Tasks.Task Get_Task_Fails_When_Task_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementMock = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var sut = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementMock.Object, teamServiceStub);

            taskRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync((DAL.Entities.Task)null);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.GetTaskById(It.IsAny<int>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Reassign_Success_When_User_And_Task_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementMock = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var sut = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementMock.Object, teamServiceStub);

            taskRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync(new DAL.Entities.Task());
            userManagerMock.Setup(um => um.FindByUserNameAsync(It.IsAny<string>())).ReturnsAsync(new User());

            await sut.Reassign(It.IsAny<int>(), It.IsAny<string>());

            taskRepositoryMock.Verify(tr => tr.Update(), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Reassign_Fails_When_Task_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementMock = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var sut = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementMock.Object, teamServiceStub);

            taskRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync((DAL.Entities.Task)null);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.Reassign(It.IsAny<int>(), It.IsAny<string>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Reassign_Fails_When_User_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementMock = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var sut = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementMock.Object, teamServiceStub);

            taskRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync(new DAL.Entities.Task());
            userManagerMock.Setup(um => um.FindByUserNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.Reassign(It.IsAny<int>(), It.IsAny<string>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task List_Tasks_For_Users_Returns_Collection_Of_Tasks_For_Current_User()
        {
            var userManagerMock = new Mock<IUserManager>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementMock = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var sut = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementMock.Object, teamServiceStub);

            List<DAL.Entities.Task> tasks = new List<DAL.Entities.Task>()
            {
                new DAL.Entities.Task() {Name = "fake01"},
                new DAL.Entities.Task() {Name = "fake02"}
            };

            taskManagementMock.Setup(tm => tm.GetMyTasks(It.IsAny<string>())).ReturnsAsync(tasks);

            var result = await sut.List(It.IsAny<string>());

            Assert.Equal(result, tasks);
        }

    }
}
