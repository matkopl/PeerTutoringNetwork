using System.Collections.Generic;
using System.Linq;
using System.Text;
using BL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeerTutoringNetwork.Controllers;
using PeerTutoringNetwork.Viewmodels;
using Xunit;

namespace Matko.Tests
{
    public class MentorDashboardControllerTests
    {
        private readonly PeerTutoringNetworkContext _context;
        private readonly MentorDashboardController _controller;

        public MentorDashboardControllerTests()
        {
            // Set up the in-memory database
            var options = new DbContextOptionsBuilder<PeerTutoringNetworkContext>()
                .UseInMemoryDatabase(databaseName: "TestMentorDashboard")
                .Options;

            _context = new PeerTutoringNetworkContext(options);

            // Seed the database with initial data
            SeedDatabase();

            // Initialize the controller
            _controller = new MentorDashboardController(_context);
        }

        private void SeedDatabase()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 201, // Unique ID
                    Username = "Student1",
                    Email = "student1@example.com",
                    FirstName = "Student",
                    LastName = "One",
                    PwdHash = Encoding.UTF8.GetBytes("student1pwd"),
                    PwdSalt = Encoding.UTF8.GetBytes("student1salt"),
                    RoleId = 2
                },
                new User
                {
                    UserId = 202, // Unique ID
                    Username = "Mentor1",
                    Email = "mentor1@example.com",
                    FirstName = "Mentor",
                    LastName = "One",
                    PwdHash = Encoding.UTF8.GetBytes("mentor1pwd"),
                    PwdSalt = Encoding.UTF8.GetBytes("mentor1salt"),
                    RoleId = 1
                }
            };

            var subjects = new List<Subject>
            {
                new Subject { SubjectId = 301, SubjectName = "Mathematics" },
                new Subject { SubjectId = 302, SubjectName = "Physics" },
                new Subject { SubjectId = 303, SubjectName = "Biology" }
            };

            var appointments = new List<Appointment>
            {
                new Appointment { AppointmentId = 401, MentorId = 201, SubjectId = 301, AppointmentDate = System.DateTime.Now.AddDays(-1) },
                new Appointment { AppointmentId = 402, MentorId = 201, SubjectId = 302, AppointmentDate = System.DateTime.Now.AddDays(-2) },
                new Appointment { AppointmentId = 403, MentorId = 201, SubjectId = 303, AppointmentDate = System.DateTime.Now.AddDays(-3) }
            };

            _context.Users.AddRange(users);
            _context.Subjects.AddRange(subjects);
            _context.Appointments.AddRange(appointments);
            _context.SaveChanges();
        }

        [Fact]
        public void Index_ReturnsViewResult_WithDashboardData()
        {
            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<MentorDashboardVM>(result.Model);

            Assert.Equal(3, model.TotalAppointments);
            Assert.Equal(3, model.TotalSubjects);

            Assert.Equal(3, model.RecentAppointments.Count); // Seeded 3 appointments
            Assert.Equal(3, model.RecentSubjects.Count); // Seeded 3 subjects

            Assert.Equal(401, model.RecentAppointments.First().AppointmentId); // Most recent appointment
            Assert.Equal(303, model.RecentSubjects.First().SubjectId); // Most recent subject
        }
    }
}
