using Xunit;
using BL.Managers;
using System.Collections.Generic;

namespace Tests.AntonioTest.Service
{
    public class SessionManagerTests
    {


        [Fact]
        public void RemoveSession_ShouldRemoveSessionCorrectly()
        {
            // Arrange
            var sessionManager = SessionManager.Instance;
            var userId = 1;
            var token = "sampleToken";
            sessionManager.AddSession(userId, token);

            // Act
            sessionManager.RemoveSession(userId);
            var session = sessionManager.GetSession(userId);

            // Assert
            Assert.Null(session); 
        }

        [Fact]
        public void SingletonInstance_ShouldAlwaysReturnSameInstance()
        {
            // Arrange
            var instance1 = SessionManager.Instance;
            var instance2 = SessionManager.Instance;

            // Assert
            Assert.Same(instance1, instance2); 
        }

        [Fact]
        public void AddSession_ShouldNotOverwriteExistingSession()
        {
            // Arrange
            var sessionManager = SessionManager.Instance;
            var userId = 1;
            var token1 = "token1";
            var token2 = "token2";

            // Act
            sessionManager.AddSession(userId, token1);
            sessionManager.AddSession(userId, token2); 

            // Assert
            var session = sessionManager.GetSession(userId);
            Assert.Equal(token1, session); 
        }

        [Fact]
        public void GetActiveUsers_ShouldReturnListOfActiveUsers()
        {
            // Arrange
            var sessionManager = SessionManager.Instance;
            sessionManager.AddSession(1, "token1");
            sessionManager.AddSession(2, "token2");

            // Act
            var activeUsers = sessionManager.GetActiveUsers();

            // Assert
            Assert.Contains(1, activeUsers);
            Assert.Contains(2, activeUsers);
            Assert.Equal(2, activeUsers.Count);
        }
    }
}
