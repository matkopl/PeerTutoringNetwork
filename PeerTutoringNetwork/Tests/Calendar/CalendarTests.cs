using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeerTutoringNetwork.Controllers;
using PeerTutoringNetwork.Viewmodels;
using Xunit;

namespace Tests
{
    public class CalendarIntegrationTests : IClassFixture<TestFixture>
    {
        private readonly PeerTutoringNetworkContext _context;
        private readonly IAppointmentService _appointmentService;
        private readonly AppointmentsController _controller;

        public CalendarIntegrationTests(TestFixture fixture)
        {
            _context = fixture.Context;
            _appointmentService = fixture.AppointmentService;
            _controller = fixture.Controller;
        }

        [Fact]
        public async Task Calendar_ReturnsViewResult_WithListOfAppointments()
        {
            // Act
            var result = await _controller.Calendar();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<AppointmentVM>>(viewResult.ViewData.Model);
            Assert.Equal(3, model.Count);
        }

        [Fact]
        public async Task DeleteAppointment_ReturnsFalse_WhenAppointmentNotFound()
        {
            // Act
            var result = await _appointmentService.DeleteAppointment(4);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteConfirmed_RedirectsToIndex_WhenAppointmentDeleted()
        {
            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Null(await _context.Appointments.FindAsync(1));
        }
    }

    public class TestFixture
    {
        public PeerTutoringNetworkContext Context { get; }
        public IAppointmentService AppointmentService { get; }
        public AppointmentsController Controller { get; }

        public TestFixture()
        {
            var options = new DbContextOptionsBuilder<PeerTutoringNetworkContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            Context = new PeerTutoringNetworkContext(options);
            AppointmentService = new AppointmentService(Context);
            Controller = new AppointmentsController(Context, AppointmentService);

            SeedDatabase();
        }

        private void  SeedDatabase()
        {
            var roles = new List<Role>
            {
                new Role { RoleId = 1, RoleName = "Mentor" },
                new Role { RoleId = 2, RoleName = "Student" }
            };

            var users = new List<User>
            {
                new User
                {
                    UserId = 1, Username = "test1", RoleId = 1, Email = "mail1@mail.com", FirstName = "test1", LastName = "test1",
                    PwdHash = Encoding.UTF8.GetBytes("0987654321"), PwdSalt = Encoding.UTF8.GetBytes("1234567890")
                },
                new User
                {
                    UserId = 2, Username = "test2", RoleId = 2, Email = "mail2@mail.com", FirstName = "test2", LastName = "test2",
                    PwdHash = Encoding.UTF8.GetBytes("1234567890"), PwdSalt = Encoding.UTF8.GetBytes("0987654321")
                }
            };

            var subjects = new List<Subject>
            {
                new Subject { SubjectId = 1, SubjectName = "Science" },
                new Subject { SubjectId = 2, SubjectName = "Math" },
                new Subject { SubjectId = 3, SubjectName = "Biology" }
            };

            var appointments = new List<Appointment>
            {
                new Appointment
                {
                    AppointmentId = 1, MentorId = 1, SubjectId = 1, AppointmentDate = DateTime.Now
                },
                new Appointment
                {
                    AppointmentId = 2, MentorId = 1, SubjectId = 2, AppointmentDate = DateTime.Now
                },
                new Appointment
                {
                    AppointmentId = 3, MentorId = 2, SubjectId = 3, AppointmentDate = DateTime.Now
                }
            };

            Context.Roles.AddRange(roles);
            Context.Users.AddRange(users);
            Context.Subjects.AddRange(subjects);
            Context.Appointments.AddRange(appointments);
            Context.SaveChanges();
        }
    }
}