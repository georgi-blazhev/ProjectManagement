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
    public class WorkLogServiceTests
    {
        [Fact]
        public async System.Threading.Tasks.Task Create_Work_Log_Success_When_Valid()
        {
            var userManagerMock = new Mock<IUserManager>();

            var workLogRepository = new Mock<IRepository<WorkLog>>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementRepository = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var taskServiceStub = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementRepository.Object, teamServiceStub);

            var sut = new WorkLogService(userManagerMock.Object, workLogRepository.Object, taskServiceStub);

            User current = new User();
            DAL.Entities.Task task = new DAL.Entities.Task() { AssigneeId = current.Id };

            List<string> roles = new List<string>();
            roles.Add("Admin");

            taskRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync(task);
            userManagerMock.Setup(um => um.GetUserRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);

            await sut.CreateWorkLog(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>());

            workLogRepository.Verify(wr => wr.Create(It.IsAny<WorkLog>()), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_Work_Log_Fails_When_Task_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();

            var workLogRepository = new Mock<IRepository<WorkLog>>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementRepository = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var taskServiceStub = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementRepository.Object, teamServiceStub);

            var sut = new WorkLogService(userManagerMock.Object, workLogRepository.Object, taskServiceStub);

            taskRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync((DAL.Entities.Task)null);

            await Assert.ThrowsAsync<NotExistingException>(() =>
            sut.CreateWorkLog(It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<string>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_Work_Log_Fails_When_Current_User_Is_Not_Assignee()
        {
            var userManagerMock = new Mock<IUserManager>();

            var workLogRepository = new Mock<IRepository<WorkLog>>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementRepository = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var taskServiceStub = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementRepository.Object, teamServiceStub);

            var sut = new WorkLogService(userManagerMock.Object, workLogRepository.Object, taskServiceStub);

            DAL.Entities.Task task = new DAL.Entities.Task() { AssigneeId = "fake-id" };

            User current = new User();

            taskRepositoryMock.Setup(tr => tr.Get(task.Id)).ReturnsAsync(task);
            userManagerMock.Setup(um => um.FindByIdAsync(current.Id)).ReturnsAsync(new User());

            List<string> roles = new List<string>();
            roles.Add("not admin");
            userManagerMock.Setup(um => um.GetUserRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            sut.CreateWorkLog(It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<string>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_Work_Log_Fails_When_Current_User_Is_Not_Admin()
        {
            var userManagerMock = new Mock<IUserManager>();

            var workLogRepository = new Mock<IRepository<WorkLog>>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementRepository = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var taskServiceStub = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementRepository.Object, teamServiceStub);

            var sut = new WorkLogService(userManagerMock.Object, workLogRepository.Object, taskServiceStub);

            User current = new User();

            DAL.Entities.Task task = new DAL.Entities.Task() { AssigneeId = current.Id };

            taskRepositoryMock.Setup(tr => tr.Get(task.Id)).ReturnsAsync(task);
            userManagerMock.Setup(um => um.FindByIdAsync(current.Id)).ReturnsAsync(new User());

            List<string> roles = new List<string>();
            roles.Add("not admin");
            userManagerMock.Setup(um => um.GetUserRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            sut.CreateWorkLog(It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<string>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Delete_WorkLog_Success_When_WorkLog_Exists()
        {
            var userManagerMock = new Mock<IUserManager>();

            var workLogRepository = new Mock<IRepository<WorkLog>>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementRepository = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var taskServiceStub = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementRepository.Object, teamServiceStub);

            var sut = new WorkLogService(userManagerMock.Object, workLogRepository.Object, taskServiceStub);

            workLogRepository.Setup(wr => wr.Get(It.IsAny<int>())).ReturnsAsync(new WorkLog());

            await sut.DeleteWorkLog(It.IsAny<int>());

            workLogRepository.Verify(wr => wr.Delete(It.IsAny<WorkLog>()), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Delete_WorkLog_Fails_When_WorkLog_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();

            var workLogRepository = new Mock<IRepository<WorkLog>>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementRepository = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var taskServiceStub = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementRepository.Object, teamServiceStub);

            var sut = new WorkLogService(userManagerMock.Object, workLogRepository.Object, taskServiceStub);

            workLogRepository.Setup(wr => wr.Get(It.IsAny<int>())).ReturnsAsync((WorkLog)null);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.DeleteWorkLog(It.IsAny<int>()));        
        }

        [Fact]
        public async System.Threading.Tasks.Task Update_WorkLog_Success_When_WorkLog_Exists()
        {
            var userManagerMock = new Mock<IUserManager>();

            var workLogRepository = new Mock<IRepository<WorkLog>>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementRepository = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var taskServiceStub = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementRepository.Object, teamServiceStub);

            var sut = new WorkLogService(userManagerMock.Object, workLogRepository.Object, taskServiceStub);

            workLogRepository.Setup(wr => wr.Get(It.IsAny<int>())).ReturnsAsync(new WorkLog());

            await sut.UpdateWorkLog(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            workLogRepository.Verify(wr => wr.Update(), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Update_WorkLog_Fails_When_WorkLog_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();

            var workLogRepository = new Mock<IRepository<WorkLog>>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementRepository = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var taskServiceStub = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementRepository.Object, teamServiceStub);

            var sut = new WorkLogService(userManagerMock.Object, workLogRepository.Object, taskServiceStub);

            workLogRepository.Setup(wr => wr.Get(It.IsAny<int>())).ReturnsAsync((WorkLog)null);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.UpdateWorkLog(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Get_WorkLog_By_Id_Returns_Task_From_Repository_When_WorkLog_Exists()
        {
            var userManagerMock = new Mock<IUserManager>();

            var workLogRepository = new Mock<IRepository<WorkLog>>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementRepository = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var taskServiceStub = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementRepository.Object, teamServiceStub);

            var sut = new WorkLogService(userManagerMock.Object, workLogRepository.Object, taskServiceStub);

            WorkLog workLog = new WorkLog();

            workLogRepository.Setup(wr => wr.Get(It.IsAny<int>())).ReturnsAsync(workLog);

            var result = await sut.GetWorkLog(It.IsAny<int>());

            Assert.Equal(result, workLog);
        }

        [Fact]
        public async System.Threading.Tasks.Task Get_WorkLog_By_Id_Fails_When_WorkLog_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();

            var workLogRepository = new Mock<IRepository<WorkLog>>();

            var teamRepositoryMock = new Mock<IRepository<Team>>();
            var teamManagementRepositoryMock = new Mock<ITeamManagementRepository<Team>>();
            var teamServiceStub = new TeamService(teamRepositoryMock.Object, userManagerMock.Object, teamManagementRepositoryMock.Object);

            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var projectServiceStub = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            var taskRepositoryMock = new Mock<IRepository<DAL.Entities.Task>>();
            var taskManagementRepository = new Mock<ITaskManagementRepository<DAL.Entities.Task>>();
            var taskServiceStub = new TaskService(userManagerMock.Object, taskRepositoryMock.Object,
                projectServiceStub, taskManagementRepository.Object, teamServiceStub);

            var sut = new WorkLogService(userManagerMock.Object, workLogRepository.Object, taskServiceStub);

            workLogRepository.Setup(wr => wr.Get(It.IsAny<int>())).ReturnsAsync((WorkLog)null);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.GetWorkLog(It.IsAny<int>()));
        }
    }
}
