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
           
            _contextMock = new Mock<PeerTutoringNetworkContext>();
            _sessionManagerMock = new Mock<SessionManager>();
            _jwtTokenProviderMock = new Mock<JwtTokenProvider>();

           
            _adminPanelService = new AdminPanelService(_contextMock.Object);

          
            _user = new User
            {
                UserId = 1,
                Username = "testUser",
                PwdHash = new byte[] { 1, 2, 3, 4 },  
                PwdSalt = new byte[] { 5, 6, 7, 8 }
            };

           
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
            Assert.False(result);  
            Assert.Null(authenticatedUser); 
        }
    }
}
