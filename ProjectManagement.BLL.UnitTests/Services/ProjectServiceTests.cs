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
    public class ProjectServiceTests
    {
        [Fact]
        public async System.Threading.Tasks.Task Create_Project_Success_When_Project_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();
            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            var sut = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<Func<Project, bool>>())).Returns((Project)null);

            await sut.CreateProjectAsync(It.IsAny<string>(), It.IsAny<string>());

            projectRepositoryMock.Verify(pr => pr.Create(It.IsAny<Project>()), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_Project_Fails_When_Project_Exists()
        {
            var userManagerMock = new Mock<IUserManager>();
            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            var sut = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            Project project = new Project();

            projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<Func<Project, bool>>())).Returns(project);

            await Assert.ThrowsAsync<AlreadyExistingException>(() => sut.CreateProjectAsync(It.IsAny<string>(), It.IsAny<string>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Delete_Project_Success_When_Project_Exists()
        {
            var userManagerMock = new Mock<IUserManager>();
            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            var sut = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            Project project = new Project();

            projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<int>())).ReturnsAsync(project);

            await sut.DeleteProjectAsync(project.Id);

            projectRepositoryMock.Verify(pr => pr.Delete(project), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Delete_Project_Fails_When_Project_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();
            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            var sut = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<int>())).ReturnsAsync((Project)null);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.DeleteProjectAsync(It.IsAny<int>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Assign_Team_To_Task_Success_When_Project_And_Team_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();
            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            var sut = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<int>())).ReturnsAsync(new Project());
            teamRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync(new Team());
            projectManagementRepositoryMock.Setup(pmr => pmr.GetProjectTeam(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((ProjectTeam)null);

            await sut.AssignTeam(It.IsAny<int>(), It.IsAny<int>());

            projectManagementRepositoryMock.Verify(pmr => pmr.AssignTeamToProject(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Assign_Team_To_Task_Fails_When_Project_And_Team_Exist_But_Team_Is_Already_Assigned()
        {
            var userManagerMock = new Mock<IUserManager>();
            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            var sut = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<int>())).ReturnsAsync(new Project());
            teamRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync(new Team());
            projectManagementRepositoryMock.Setup(pmr => pmr.GetProjectTeam(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ProjectTeam());

            await Assert.ThrowsAsync<AlreadyExistingException>(() => sut.AssignTeam(It.IsAny<int>(), It.IsAny<int>())); 
        }

        [Fact]
        public async System.Threading.Tasks.Task Assign_Team_To_Task_Fails_When_Project_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();
            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            var sut = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<int>())).ReturnsAsync((Project)null);
            
            await Assert.ThrowsAsync<NotExistingException>(() => sut.AssignTeam(It.IsAny<int>(), It.IsAny<int>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Assign_Team_To_Task_Fails_When_Team_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();
            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            var sut = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<int>())).ReturnsAsync(new Project());
            teamRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>())).ReturnsAsync((Team)null);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.AssignTeam(It.IsAny<int>(), It.IsAny<int>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Update_Project_Success_When_Project_Exists()
        {
            var userManagerMock = new Mock<IUserManager>();
            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            var sut = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            Project project = new Project();

            projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<int>())).ReturnsAsync(project);

            await sut.UpdateProjectAsync(project.Id, It.IsAny<string>());

            projectRepositoryMock.Verify(pr => pr.Update(), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task Update_Project_Fails_When_Project_Does_Not_Exist()
        {
            var userManagerMock = new Mock<IUserManager>();
            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            var sut = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<int>())).ReturnsAsync((Project)null);

            await Assert.ThrowsAsync<NotExistingException>(() => sut.UpdateProjectAsync(It.IsAny<int>(), It.IsAny<string>()));
        }

        [Fact]
        public async System.Threading.Tasks.Task Get_Project_By_Id_Success_When_Project_Exists()
        {
            var userManagerMock = new Mock<IUserManager>();
            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            var sut = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            Project project = new Project();

            projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<int>())).ReturnsAsync(project);

            await sut.GetProjectById(It.IsAny<int>());

            projectRepositoryMock.Verify(pr => pr.Get(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async System.Threading.Tasks.Task List_All_Projects_Returns_Collection_From_Repository()
        {
            var userManagerMock = new Mock<IUserManager>();
            var projectRepositoryMock = new Mock<IRepository<Project>>();
            var projectManagementRepositoryMock = new Mock<IProjectsManagementRepository<Project>>();
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            var sut = new ProjectService(projectRepositoryMock.Object, userManagerMock.Object,
                projectManagementRepositoryMock.Object, teamRepositoryMock.Object);

            List<Project> projects = new List<Project>()
            {
                new Project() { Name = "fake01"},
                new Project() { Name = "fake02"}
            };

            projectRepositoryMock.Setup(pr => pr.All()).ReturnsAsync(projects);

            var result = await sut.List();

            Assert.Equal(result, projects);
        }
    }
}
