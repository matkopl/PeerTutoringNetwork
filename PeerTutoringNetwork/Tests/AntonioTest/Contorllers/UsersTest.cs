using Moq;
using Xunit;
using PeerTutoringNetwork.Controllers;
using BL.Models;
using PeerTutoringNetwork.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Tests.AntonioTest.Contorllers
{
    public class UsersControllerTests
    {
        private readonly Mock<PeerTutoringNetworkContext> _contextMock;
        private readonly UsersController _usersController;

        public UsersControllerTests()
        {
            _contextMock = new Mock<PeerTutoringNetworkContext>();
            var user = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com"
            };

            var users = new List<User> { user }.AsQueryable();
            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _contextMock.Setup(c => c.Users).Returns(mockDbSet.Object);
            _usersController = new UsersController(null, _contextMock.Object);
        }

        [Fact]
        public void AddUser_ShouldReturnOk_WhenUserIsAddedSuccessfully()
        {
            // Arrange
            var userDto = new UserRegisterDto
            {
                Id = 2,
                Username = "newUser",
                Email = "newuser@example.com",
                Password = "password123",
                RoleId = 1
            };

            // Act
            var result = _usersController.AddUser(userDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User added successfully", okResult.Value);
        }

    }
}
