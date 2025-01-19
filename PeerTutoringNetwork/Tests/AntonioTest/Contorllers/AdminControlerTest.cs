using Moq;
using Xunit;
using PeerTutoringNetwork.Controllers;
using Microsoft.AspNetCore.Mvc;
using BL.Models;
using PeerTutoringNetwork.DTOs;
using PeerTutoringNetwork;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PeerTutoringNetwork.DTO;

namespace Tests.AntonioTest.Contorllers
{
    public class AdminControllerTests
    {
        private readonly Mock<PeerTutoringNetworkContext> _contextMock;
        private readonly AdminController _adminController;

        public AdminControllerTests()
        {
           
            _contextMock = new Mock<PeerTutoringNetworkContext>();

           
            _adminController = new AdminController(null, _contextMock.Object);

         
            var users = new List<User>
            {
                new User { UserId = 1, Username = "testUser1", RoleId = 3, Email = "test1@example.com" },
                new User { UserId = 2, Username = "testUser2", RoleId = 2, Email = "test2@example.com" },
                new User { UserId = 3, Username = "testUser3", RoleId = 1, Email = "test3@example.com" }
            }.AsQueryable();

            var subjects = new List<Subject>
            {
                new Subject { SubjectId = 1, SubjectName = "Math", Description = "Mathematics subject" },
                new Subject { SubjectId = 2, SubjectName = "English", Description = "English subject" }
            }.AsQueryable();

            var mockUsersDbSet = new Mock<DbSet<User>>();
            var mockSubjectsDbSet = new Mock<DbSet<Subject>>();

            mockUsersDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUsersDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUsersDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUsersDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            mockSubjectsDbSet.As<IQueryable<Subject>>().Setup(m => m.Provider).Returns(subjects.Provider);
            mockSubjectsDbSet.As<IQueryable<Subject>>().Setup(m => m.Expression).Returns(subjects.Expression);
            mockSubjectsDbSet.As<IQueryable<Subject>>().Setup(m => m.ElementType).Returns(subjects.ElementType);
            mockSubjectsDbSet.As<IQueryable<Subject>>().Setup(m => m.GetEnumerator()).Returns(subjects.GetEnumerator());

            _contextMock.Setup(c => c.Users).Returns(mockUsersDbSet.Object);
            _contextMock.Setup(c => c.Subjects).Returns(mockSubjectsDbSet.Object);
        }

        [Fact]
        public void GetUserById_ShouldReturnUser_WhenUserExists()
        {
            // Act
            var result = _adminController.GetUserById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var user = Assert.IsType<UserProfileDto>(okResult.Value);
            Assert.Equal(1, user.UserId);
        }

        [Fact]
        public void GetUserById_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Act
            var result = _adminController.GetUserById(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void DeleteUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Act
            var result = _adminController.DeleteUser(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
      
    }
}
