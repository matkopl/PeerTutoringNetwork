using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using PeerTutoringNetwork.Controllers;
using BL.Models;
using PeerTutoringNetwork.DTO;
using PeerTutoringNetwork.Security;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using BL.Models;
using PeerTutoringNetwork.DTOs;

namespace Tests.AntonioTest.Contorllers
{
    public class UserControllerTests
    {
        private readonly Mock<PeerTutoringNetworkContext> _contextMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _contextMock = new Mock<PeerTutoringNetworkContext>();
            _configurationMock = new Mock<IConfiguration>();


            var b64salt = PasswordHashProvider.GetSalt();
            var b64hash = PasswordHashProvider.GetHash("testpassword", b64salt);

         
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Username = "testuser",
                    PwdHash = Convert.FromBase64String(b64hash),
                    PwdSalt = Convert.FromBase64String(b64salt),
                    Email = "testuser@example.com",
                    RoleId = 1
                }
            }.AsQueryable();

            var usersDbSetMock = new Mock<DbSet<User>>();
            usersDbSetMock.As<IQueryable<User>>()
                .Setup(m => m.Provider).Returns(users.Provider);
            usersDbSetMock.As<IQueryable<User>>()
                .Setup(m => m.Expression).Returns(users.Expression);
            usersDbSetMock.As<IQueryable<User>>()
                .Setup(m => m.ElementType).Returns(users.ElementType);
            usersDbSetMock.As<IQueryable<User>>()
                .Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _contextMock.Setup(c => c.Users).Returns(usersDbSetMock.Object);

            _userController = new UserController(_configurationMock.Object, _contextMock.Object);
        }


        [Fact]
        public void UpdateProfile_ShouldReturnOk_WhenUserProfileIsUpdated()
        {
            // Arrange
            var updateDto = new UserProfileUpdateDto
            {
                UserId = 1,
                FirstName = "Updated",
                LastName = "User",
                Email = "updated@example.com",
                Phone = "123456789"
            };

            // Act
            var result = _userController.UpdateProfile(updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Profile updated successfully", okResult.Value);
        }

        [Fact]
        public void DeleteUser_ShouldReturnOk_WhenUserDeleted()
        {
            // Arrange
            var userId = 1;
            _contextMock.Setup(c => c.Users.Find(It.IsAny<int>())).Returns(new User { UserId = userId });

            // Act
            var result = _userController.DeleteUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User deleted successfully", okResult.Value);
        }



    }
}
