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
    public class SubjectsControllerTests
    {
        private readonly PeerTutoringNetworkContext _context;
        private readonly SubjectsController _controller;

        public SubjectsControllerTests()
        {
            // Set up the in-memory database
            var options = new DbContextOptionsBuilder<PeerTutoringNetworkContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PeerTutoringNetworkContext(options);

            // Clear existing data to avoid conflicts
            _context.Database.EnsureDeleted();

            // Seed the database with initial data
            SeedDatabase();

            // Initialize the controller
            _controller = new SubjectsController(_context);
        }

        private void SeedDatabase()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.Appointments.RemoveRange(_context.Appointments);
            _context.AppointmentReservations.RemoveRange(_context.AppointmentReservations);
            _context.SaveChanges();

            var users = new List<User>
            {
                new User
                {
                    UserId = 101,
                    Username = "AdminUser",
                    Email = "admin@example.com",
                    FirstName = "Admin",
                    LastName = "User",
                    PwdHash = Encoding.UTF8.GetBytes("adminpassword"),
                    PwdSalt = Encoding.UTF8.GetBytes("adminsalthash"),
                    RoleId = 1
                },
                new User
                {
                    UserId = 102,
                    Username = "StudentUser",
                    Email = "student@example.com",
                    FirstName = "Student",
                    LastName = "User",
                    PwdHash = Encoding.UTF8.GetBytes("studentpassword"),
                    PwdSalt = Encoding.UTF8.GetBytes("studentsalthash"),
                    RoleId = 2
                }
            };

            var subjects = new List<Subject>
            {
                new Subject
                {
                    SubjectId = 201,
                    SubjectName = "Mathematics",
                    Description = "Advanced Mathematics",
                    CreatedByUserId = 101
                },
                new Subject
                {
                    SubjectId = 202,
                    SubjectName = "Physics",
                    Description = "Physics Fundamentals",
                    CreatedByUserId = 102
                }
            };

            _context.Users.AddRange(users);
            _context.Subjects.AddRange(subjects);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfSubjects()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<SubjectVM>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Create_ValidModel_AddsSubject()
        {
            // Arrange
            var subjectVM = new SubjectVM
            {
                SubjectName = "Biology",
                Description = "Life Science",
                CreatedByUserId = 101
            };

            // Act
            var result = await _controller.Create(subjectVM);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            var subjects = _context.Subjects.ToList();
            Assert.Equal(3, subjects.Count);
            Assert.Equal("Biology", subjects.Last().SubjectName);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesSubject()
        {
            // Act
            var result = await _controller.DeleteConfirmed(201);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            var subjects = _context.Subjects.ToList();
            Assert.Single(subjects);
            Assert.DoesNotContain(subjects, s => s.SubjectId == 201);
        }

        [Fact]
        public async Task Edit_ValidModel_UpdatesSubject()
        {
            // Arrange
            // Detach all tracked entities to avoid EF Core tracking conflicts
            foreach (var entry in _context.ChangeTracker.Entries().ToList())
            {
                entry.State = EntityState.Detached;
            }

            var subjectVM = new SubjectVM
            {
                SubjectId = 201, // ID of the existing subject in SeedDatabase
                SubjectName = "Updated Math",
                Description = "Updated Mathematics",
                CreatedByUserId = 101
            };

            // Act
            var result = await _controller.Edit(subjectVM.SubjectId, subjectVM);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            var updatedSubject = await _context.Subjects.FindAsync(subjectVM.SubjectId);
            Assert.NotNull(updatedSubject);
            Assert.Equal("Updated Math", updatedSubject.SubjectName);
            Assert.Equal("Updated Mathematics", updatedSubject.Description);
        }

        [Fact]
        public async Task Details_ReturnsSubjectDetails()
        {
            // Act
            var result = await _controller.Details(201);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<SubjectVM>(viewResult.ViewData.Model);
            Assert.Equal(201, model.SubjectId);
            Assert.Equal("Mathematics", model.SubjectName);
        }
    }
}
