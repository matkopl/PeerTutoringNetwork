using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PeerTutoringNetwork.Controllers;
using PeerTutoringNetwork.Viewmodels;
using Xunit;

namespace Tests
{
    public class CalendarTests
    {
        private readonly Mock<IAppointmentService> _mockAppointmentService;
        private readonly Mock<PeerTutoringNetworkContext> _mockContext;
        private readonly AppointmentsController _controller;

        public CalendarTests()
        {
            _mockAppointmentService = new Mock<IAppointmentService>();
            _mockContext = new Mock<PeerTutoringNetworkContext>();
            _controller = new AppointmentsController(_mockContext.Object, _mockAppointmentService.Object);
        }

        [Fact]
        public async Task Calendar_ReturnsViewResult_WithListOfAppointments()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment { AppointmentId = 1, MentorId = 1, SubjectId = 1, Mentor = new User { Username = "Mentor1" }, Subject = new Subject { SubjectName = "Math" }, AppointmentDate = DateTime.Now },
                new Appointment { AppointmentId = 2, MentorId = 2, SubjectId = 2, Mentor = new User { Username = "Mentor2" }, Subject = new Subject { SubjectName = "Science" }, AppointmentDate = DateTime.Now }
            };
            _mockAppointmentService.Setup(service => service.GetAppointments()).ReturnsAsync(appointments);

            // Act
            var result = await _controller.Calendar();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<AppointmentVM>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirectToActionResult_WhenAppointmentDeleted()
        {
            // Arrange
            int appointmentId = 1;
            _mockAppointmentService.Setup(service => service.DeleteAppointment(appointmentId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteConfirmed(appointmentId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsNotFoundResult_WhenAppointmentNotDeleted()
        {
            // Arrange
            int appointmentId = 1;
            _mockAppointmentService.Setup(service => service.DeleteAppointment(appointmentId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteConfirmed(appointmentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}