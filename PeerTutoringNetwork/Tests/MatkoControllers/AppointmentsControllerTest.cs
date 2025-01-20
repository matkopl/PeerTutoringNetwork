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

namespace Matko.Tests
{
    public class AppointmentsControllerTests
    {
        private readonly PeerTutoringNetworkContext _context;
        private readonly AppointmentsController _controller;

        public AppointmentsControllerTests()
        {
            // Set up the in-memory database
            var options = new DbContextOptionsBuilder<PeerTutoringNetworkContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PeerTutoringNetworkContext(options);

            // Seed the database with initial data
            SeedDatabase();

            // Initialize the controller
            _controller = new AppointmentsController(_context, new AppointmentService(_context));
        }

        private void SeedDatabase()
        {
            var users = new List<User>
            {
                new User 
                { 
                    UserId = 201, 
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
                    UserId = 202, 
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
                new Subject { SubjectId = 301, SubjectName = "Mathematics" },
                new Subject { SubjectId = 302, SubjectName = "Physics" }
            };

            var appointments = new List<Appointment>
            {
                new Appointment { AppointmentId = 601, MentorId = 201, SubjectId = 301, AppointmentDate = System.DateTime.Now.AddDays(1) },
                new Appointment { AppointmentId = 402, MentorId = 201, SubjectId = 302, AppointmentDate = System.DateTime.Now.AddDays(2) }
            };

            _context.Users.AddRange(users);
            _context.Subjects.AddRange(subjects);
            _context.Appointments.AddRange(appointments);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfAppointments()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<AppointmentVM>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Create_ValidModel_AddsAppointment()
        {
            // Arrange
            var appointmentVM = new AppointmentVM
            {
                MentorId = 201,
                SubjectId = 301,
                AppointmentDate = System.DateTime.Now.AddDays(3)
            };

            // Act
            var result = await _controller.Create(appointmentVM);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            var appointments = _context.Appointments.ToList();
            Assert.Equal(3, appointments.Count);
        }

        [Fact]
        public async Task Edit_ValidModel_UpdatesAppointment()
        {
            // Arrange
            var appointmentId = 601; // Use an existing valid AppointmentId
            var appointmentVM = new AppointmentVM
            {
                AppointmentId = appointmentId, // Ensure this ID exists in the seeded data
                MentorId = 501, // Ensure this matches an existing Mentor ID in the seed
                SubjectId = 401, // Ensure this matches an existing Subject ID in the seed
                AppointmentDate = DateTime.Now.AddDays(1)
            };

            // Detach any tracked entity to avoid conflicts
            var trackedEntity = _context.ChangeTracker.Entries<Appointment>().FirstOrDefault(e => e.Entity.AppointmentId == appointmentId);
            if (trackedEntity != null)
            {
                trackedEntity.State = EntityState.Detached;
            }

            // Act
            var result = await _controller.Edit(appointmentId, appointmentVM);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            var updatedAppointment = await _context.Appointments.FindAsync(appointmentId);
            Assert.NotNull(updatedAppointment);
            Assert.Equal(501, updatedAppointment.MentorId);
            Assert.Equal(401, updatedAppointment.SubjectId);
            Assert.Equal(appointmentVM.AppointmentDate, updatedAppointment.AppointmentDate);
        }

        [Fact]
        public async Task Details_ReturnsAppointmentDetails()
        {
            // Act
            var result = await _controller.Details(601); // Use valid AppointmentId from the seed

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AppointmentVM>(viewResult.ViewData.Model);
            Assert.Equal(601, model.AppointmentId); // Match the ID in the seed
            Assert.Equal("Mentor1", model.MentorUsername); // Match the mentor username from the seed
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WithAppointmentDetails()
        {
            // Act
            var result = await _controller.Delete(601); // Use valid AppointmentId from the seed

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AppointmentVM>(viewResult.ViewData.Model);
            Assert.Equal(601, model.AppointmentId); // Match the ID in the seed
        }
    }
}
