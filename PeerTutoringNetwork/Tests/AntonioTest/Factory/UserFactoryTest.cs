using Xunit;
using BL.Class;
using BL.Factories;
using BL.lnterfaces;
using System;

namespace Tests.AntonioTest.Factory
{
    public class UserFactoryTests
    {
        [Fact]
        public void CreateUser_ShouldReturnAdminUser_WhenRoleIs3()
        {
            // Act
            var user = UserFactory.CreateUser(3);

            // Assert
            Assert.IsType<AdminUser>(user);
        }

        [Fact]
        public void CreateUser_ShouldReturnTeacherUser_WhenRoleIs2()
        {
            // Act
            var user = UserFactory.CreateUser(2);

            // Assert
            Assert.IsType<TeacherUser>(user);
        }

        [Fact]
        public void CreateUser_ShouldReturnStudentUser_WhenRoleIs1()
        {
            // Act
            var user = UserFactory.CreateUser(1);

            // Assert
            Assert.IsType<StudentUser>(user);
        }

        [Fact]
        public void CreateUser_ShouldThrowArgumentException_WhenRoleIsInvalid()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => UserFactory.CreateUser(999));
            Assert.Equal("Invalid role Id", exception.Message);
        }
    }
}
