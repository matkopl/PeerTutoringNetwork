using Moq;
using Xunit;
using BL.Services;
using BL.Models;
using PeerTutoringNetwork.Security;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BL.Managers;

namespace Tests.AntonioTest.Service
{
    public class AdminPanelServiceTests
    {
        private readonly Mock<PeerTutoringNetworkContext> _contextMock;
        private readonly Mock<SessionManager> _sessionManagerMock;
        private readonly Mock<JwtTokenProvider> _jwtTokenProviderMock;
        private readonly AdminPanelService _adminPanelService;
        private readonly User _user;

        public AdminPanelServiceTests()
        {
            // Mock the dependencies
            _contextMock = new Mock<PeerTutoringNetworkContext>();
            _sessionManagerMock = new Mock<SessionManager>();
            _jwtTokenProviderMock = new Mock<JwtTokenProvider>();

            // Create an instance of the AdminPanelService
            _adminPanelService = new AdminPanelService(_contextMock.Object);

            // Create a dummy user for authentication
            _user = new User
            {
                UserId = 1,
                Username = "testUser",
                PwdHash = new byte[] { 1, 2, 3, 4 },  // Simulate hashed password
                PwdSalt = new byte[] { 5, 6, 7, 8 }
            };

            // Mock DbSet<User>
            var users = new List<User> { _user }.AsQueryable();
            var usersDbSet = new Mock<DbSet<User>>();
            usersDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            usersDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            usersDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _contextMock.Setup(c => c.Users).Returns(usersDbSet.Object);
        }


        [Fact]
        public void Authenticate_ShouldReturnFalse_WhenCredentialsAreInvalid()
        {
            // Arrange
            var invalidPassword = "wrongPassword";

            // Act
            var result = _adminPanelService.Authenticate(_user.Username, invalidPassword, out var authenticatedUser);

            // Assert
            Assert.False(result);  // Check that authentication failed
            Assert.Null(authenticatedUser);  // Ensure no user is authenticated
        }
    }
}
