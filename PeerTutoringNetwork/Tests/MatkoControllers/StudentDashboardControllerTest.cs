using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeerTutoringNetwork.Controllers;
using PeerTutoringNetwork.DesignPatterns;
using PeerTutoringNetwork.Viewmodels;
using Xunit;

namespace Matko.Tests
{
    public class StudentDashboardControllerTests
    {
        private readonly PeerTutoringNetworkContext _context;
        private readonly StudentDashboardController _controller;

        public StudentDashboardControllerTests()
        {
            // Set up the in-memory database
            var options = new DbContextOptionsBuilder<PeerTutoringNetworkContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PeerTutoringNetworkContext(options);
            _context.Database.EnsureDeleted();

            // Seed the database with initial data
            SeedDatabase();

            // Initialize the DashboardFacade
            var dashboardFacade = new DashboardFacade(_context);

            // Initialize the controller with the DashboardFacade
            _controller = new StudentDashboardController(dashboardFacade);
        }

        private void SeedDatabase()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 701,
                    Username = "Mentor1",
                    Email = "mentor1@example.com",
                    FirstName = "Mentor",
                    LastName = "One",
                    PwdHash = Encoding.UTF8.GetBytes("12345678"),
                    PwdSalt = Encoding.UTF8.GetBytes("12345678"),
                    RoleId = 1
                },
                new User
                {
                    UserId = 702,
                    Username = "Student1",
                    Email = "student1@example.com",
                    FirstName = "Student",
                    LastName = "One",
                    PwdHash = Encoding.UTF8.GetBytes("12345678"),
                    PwdSalt = Encoding.UTF8.GetBytes("12345678"),
                    RoleId = 2
                }
            };

            var subjects = new List<Subject>
            {
                new Subject { SubjectId = 801, SubjectName = "Mathematics" },
                new Subject { SubjectId = 802, SubjectName = "Physics" }
            };

            var appointments = new List<Appointment>
            {
                new Appointment { AppointmentId = 901, MentorId = 701, SubjectId = 801, AppointmentDate = DateTime.Now.AddDays(1) },
                new Appointment { AppointmentId = 902, MentorId = 701, SubjectId = 802, AppointmentDate = DateTime.Now.AddDays(2) }
            };

            var reservations = new List<AppointmentReservation>
            {
                new AppointmentReservation { ReservationId = 1001, AppointmentId = 901, StudentId = 702, ReservationTime = DateTime.Now }
            };

            _context.Users.AddRange(users);
            _context.Subjects.AddRange(subjects);
            _context.Appointments.AddRange(appointments);
            _context.AppointmentReservations.AddRange(reservations);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithDashboardVM()
        {
            // Arrange
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI3MDIifQ.abc123"; // Mock JWT token with userId = 702
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request = { QueryString = new QueryString("?jwtToken=" + token) }
                }
            };

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<StudentDashboardVM>(viewResult.Model);
            Assert.Single(model.Reservations);
            Assert.Single(model.AvailableAppointments);
        }

        [Fact]
        public async Task Index_RedirectsToLogin_WhenTokenIsMissing()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Login.html", redirectResult.Url);
        }

        [Fact]
        public async Task Index_ReturnsUnauthorized_WhenTokenIsInvalid()
        {
            // Arrange
            var token = "invalid.token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request = { QueryString = new QueryString("?jwtToken=" + token) }
                }
            };

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
