using Moq;
using Xunit;
using BL.Models;
using PeerTutoringNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BL.Class;

namespace Test.AntonioTest.Strategy
{
    public class TeacherActionStrategyTests
    {
        private readonly Mock<PeerTutoringNetworkContext> _contextMock;
        private readonly TeacherActionStrategy _teacherActionStrategy;

        public TeacherActionStrategyTests()
        {
            // Initialize the mock context
            _contextMock = new Mock<PeerTutoringNetworkContext>();

            // Mocking the Appointments DbSet
            var appointments = new List<Appointment>
            {
                new Appointment { AppointmentId = 1, MentorId = 1, SubjectId = 101, AppointmentDate = DateTime.Now.AddDays(1) },
                new Appointment { AppointmentId = 2, MentorId = 1, SubjectId = 102, AppointmentDate = DateTime.Now.AddDays(2) }
            }.AsQueryable();
            var appointmentsDbSetMock = new Mock<DbSet<Appointment>>();
            appointmentsDbSetMock.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(appointments.Provider);
            appointmentsDbSetMock.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(appointments.Expression);
            appointmentsDbSetMock.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(appointments.ElementType);
            appointmentsDbSetMock.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(appointments.GetEnumerator());

            // Mocking the Reviews DbSet
            var reviews = new List<Review>
            {
                new Review { ReviewId = 1, UserId = 1, SubjectId = 101, Rating = 5 },
                new Review { ReviewId = 2, UserId = 1, SubjectId = 102, Rating = 4 }
            }.AsQueryable();
            var reviewsDbSetMock = new Mock<DbSet<Review>>();
            reviewsDbSetMock.As<IQueryable<Review>>().Setup(m => m.Provider).Returns(reviews.Provider);
            reviewsDbSetMock.As<IQueryable<Review>>().Setup(m => m.Expression).Returns(reviews.Expression);
            reviewsDbSetMock.As<IQueryable<Review>>().Setup(m => m.ElementType).Returns(reviews.ElementType);
            reviewsDbSetMock.As<IQueryable<Review>>().Setup(m => m.GetEnumerator()).Returns(reviews.GetEnumerator());

            // Setting up the context mock to return the mocked DbSets
            _contextMock.Setup(c => c.Appointments).Returns(appointmentsDbSetMock.Object);
            _contextMock.Setup(c => c.Reviews).Returns(reviewsDbSetMock.Object);

            // Initialize the strategy class
            _teacherActionStrategy = new TeacherActionStrategy(_contextMock.Object);
        }      

        [Fact]
        public void ExecuteAction_ShouldLogCorrectMessages_WhenFetchingAppointmentsAndReviews()
        {
            // Arrange
            int userId = 1;

            // Capture the console output
            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);

            // Act
            _teacherActionStrategy.ExecuteAction(userId);

            // Assert
            string output = stringWriter.ToString();
            Assert.Contains("Teacher Action: Fetching appointments and reviews for User ID: 1", output);
            Assert.Contains("Appointments:", output);
            Assert.Contains("Review ID: 1, Rating: 5", output);
            Assert.Contains("Review ID: 2, Rating: 4", output);
        }
    }
}
