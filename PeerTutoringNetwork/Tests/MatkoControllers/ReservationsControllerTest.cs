using BL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeerTutoringNetwork.Controllers;
using PeerTutoringNetwork.DesignPatterns;
using PeerTutoringNetwork.Viewmodels;
using Xunit;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Matko.Tests
{
    public class ReservationsControllerTests
    {
        private readonly ReservationsController _controller;
        private readonly ReservationNotifier _reservationNotifier;
        private readonly ReservationRepository _reservationRepository;
        private readonly PeerTutoringNetworkContext _context;

        public ReservationsControllerTests()
        {
            var options = new DbContextOptionsBuilder<PeerTutoringNetworkContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PeerTutoringNetworkContext(options);
            _context.Database.EnsureDeleted();
            SeedDatabase();

            _reservationNotifier = new ReservationNotifier();
            _reservationRepository = new ReservationRepository(_context);
            var reservationFactory = new ReservationFactory();

            _controller = new ReservationsController(
                _context,
                reservationFactory,
                _reservationRepository,
                _reservationNotifier
            );
        }

        private void SeedDatabase()
        {
            var mentor = new User
            {
                UserId = 601,
                Username = "Mentor1",
                Email = "mentor1@example.com",
                FirstName = "Mentor",
                LastName = "One",
                PwdHash = Encoding.UTF8.GetBytes("12345678"),
                PwdSalt = Encoding.UTF8.GetBytes("12345678"),
                RoleId = 1
            };

            var student = new User
            {
                UserId = 602,
                Username = "Student1",
                Email = "student1@example.com",
                FirstName = "Student",
                LastName = "One",
                PwdHash = Encoding.UTF8.GetBytes("12345678"),
                PwdSalt = Encoding.UTF8.GetBytes("12345678"),
                RoleId = 2
            };

            var subject = new Subject
            {
                SubjectId = 501,
                SubjectName = "Mathematics",
                Description = "Math Subject"
            };

            var appointment = new Appointment
            {
                AppointmentId = 701,
                MentorId = mentor.UserId,
                SubjectId = subject.SubjectId,
                AppointmentDate = DateTime.Now.AddDays(1),
                Mentor = mentor,
                Subject = subject,
            };

            var reservation = new AppointmentReservation
            {
                ReservationId = 801,
                AppointmentId = appointment.AppointmentId,
                StudentId = student.UserId,
                ReservationTime = DateTime.Now,
                Appointment = appointment,
                Student = student
            };

            _context.Users.AddRange(mentor, student);
            _context.Subjects.Add(subject);
            _context.Appointments.Add(appointment);
            _context.AppointmentReservations.Add(reservation);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfReservations()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<ReservationVM>>(viewResult.ViewData.Model);
            Assert.NotEmpty(model); 
            Assert.Equal(1, model.Count); 
        }

        [Fact]
        public async Task Create_ValidModel_AddsReservation_AndNotifies()
        {
            // Arrange
            var newReservation = new ReservationVM
            {
                AppointmentId = 701, 
                StudentId = 602, 
                ReservationTime = DateTime.Now
            };

            // Act
            var result = await _controller.Create(newReservation);

            // Assert
            if (result is ConflictObjectResult conflictResult)
            {
                Assert.Equal("This appointment is already reserved.", conflictResult.Value);
            }
            else
            {
                var jsonResult = Assert.IsType<JsonResult>(result);
                Assert.Contains("Appointment reserved successfully!", jsonResult.Value.ToString());

                var reservations = await _context.AppointmentReservations.ToListAsync();
                Assert.Equal(2, reservations.Count);
            }
        }

        [Fact]
        public async Task Create_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Error", "Invalid data");

            var invalidReservation = new ReservationVM();

            var result = await _controller.Create(invalidReservation);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesReservation_AndNotifies()
        {
            // Act
            var result = await _controller.DeleteConfirmed(801); // Valid seeded reservation ID

            // Assert
            if (result is NotFoundResult)
            {
                Assert.True(false, "Reservation ID 801 not found in the test context.");
            }
            else
            {
                var redirectResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirectResult.ActionName);

                var reservations = await _context.AppointmentReservations.ToListAsync();
                Assert.Empty(reservations);
            }
        }

        [Fact]
        public async Task Details_ReturnsReservationDetails()
        {
            // Act
            var result = await _controller.Details(801); 

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ReservationVM>(viewResult.ViewData.Model);

            Assert.Equal(801, model.ReservationId); 
            Assert.Equal(602, model.StudentId); 
            Assert.Equal("Student1", model.StudentName); 
        }


        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ValidModel_UpdatesReservation()
        {
            var updatedReservation = new ReservationVM
            {
                ReservationId = 801,
                AppointmentId = 701,
                StudentId = 602,
                ReservationTime = DateTime.Now.AddDays(2)
            };

            _context.Entry(await _context.AppointmentReservations.FindAsync(801)).State = EntityState.Detached;

            var result = await _controller.Edit(801, updatedReservation);

            Assert.IsType<RedirectToActionResult>(result);

            var reservation = await _context.AppointmentReservations.FindAsync(801);
            Assert.NotNull(reservation);
            Assert.Equal(updatedReservation.ReservationTime, reservation.ReservationTime);
        }

        [Fact]
        public async Task Edit_InvalidModel_ReturnsViewResult()
        {
            _controller.ModelState.AddModelError("Error", "Invalid data");

            var invalidReservation = new ReservationVM
            {
                ReservationId = 801,
                AppointmentId = 701,
                StudentId = 602
            };

            var result = await _controller.Edit(801, invalidReservation);

            Assert.IsType<ViewResult>(result);
        }
    }
}
