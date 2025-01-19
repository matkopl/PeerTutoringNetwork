using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeerTutoringNetwork.Controllers;
using Xunit;

namespace Matko.Tests
{
    public class ReservationsControllerTests
    {
        private readonly PeerTutoringNetworkContext _context;
        private readonly ReservationsController _controller;

        public ReservationsControllerTests()
        {
            // Ensure a unique in-memory database for this test class
            var options = new DbContextOptionsBuilder<PeerTutoringNetworkContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use unique database name
                .Options;

            _context = new PeerTutoringNetworkContext(options);

            // Seed the database with test data
            SeedDatabase();

            // Initialize the controller
            _controller = new ReservationsController(_context);
        }

        private void SeedDatabase()
        {
            // Clear existing data if any (in case of overlapping seeding issues)
            _context.Users.RemoveRange(_context.Users);
            _context.Appointments.RemoveRange(_context.Appointments);
            _context.AppointmentReservations.RemoveRange(_context.AppointmentReservations);
            _context.SaveChanges();

            var users = new List<User>
            {
                new User
                {
                    UserId = 601, // Unique ID
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
                    UserId = 602, // Unique ID
                    Username = "Mentor1",
                    Email = "mentor1@example.com",
                    FirstName = "Mentor",
                    LastName = "One",
                    PwdHash = Encoding.UTF8.GetBytes("mentor1pwd"),
                    PwdSalt = Encoding.UTF8.GetBytes("mentor1salt"),
                    RoleId = 1
                }
            };

            var appointments = new List<Appointment>
            {
                new Appointment
                {
                    AppointmentId = 701, // Unique ID
                    MentorId = 602,
                    SubjectId = 501, // Ensure unique SubjectId
                    AppointmentDate = DateTime.Now.AddDays(1)
                }
            };

            var reservations = new List<AppointmentReservation>
            {
                new AppointmentReservation
                {
                    ReservationId = 801, // Unique ID
                    AppointmentId = 701,
                    StudentId = 601,
                    ReservationTime = DateTime.Now
                }
            };

            _context.Users.AddRange(users);
            _context.Appointments.AddRange(appointments);
            _context.AppointmentReservations.AddRange(reservations);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfReservations()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<AppointmentReservation>>(viewResult.ViewData.Model);
            Assert.Single(model); // Ensure there is one reservation
        }

        [Fact]
        public async Task Create_ValidModel_AddsReservation()
        {
            // Arrange
            var newReservation = new AppointmentReservation
            {
                AppointmentId = 701,
                StudentId = 601,
                ReservationTime = DateTime.Now
            };

            // Act
            var result = await _controller.Create(newReservation);

            // Assert
            var redirectResult = Assert.IsType<ViewResult>(result);
            var reservations = _context.AppointmentReservations.ToList();
            Assert.Equal(2, reservations.Count); // Ensure the new reservation is added
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesReservation()
        {
            // Act
            var result = await _controller.DeleteConfirmed(801);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Reservation canceled successfully.", okResult.Value);

            var reservations = _context.AppointmentReservations.ToList();
            Assert.Empty(reservations); // Ensure the reservation is deleted
        }

        [Fact]
        public async Task Edit_ValidModel_UpdatesReservation()
        {
            // Arrange
            var reservation = await _context.AppointmentReservations.FindAsync(801);
            Assert.NotNull(reservation);

            reservation.ReservationTime = DateTime.Now.AddDays(2); // Update a field

            // Act
            var result = await _controller.Edit(801, reservation);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            var updatedReservation = await _context.AppointmentReservations.FindAsync(801);
            Assert.NotNull(updatedReservation);
            Assert.Equal(reservation.ReservationTime, updatedReservation.ReservationTime); // Ensure the change is saved
        }

        [Fact]
        public async Task Details_ReturnsReservationDetails()
        {
            // Act
            var result = await _controller.Details(801);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AppointmentReservation>(viewResult.ViewData.Model);
            Assert.Equal(801, model.ReservationId);
        }
    }
}
