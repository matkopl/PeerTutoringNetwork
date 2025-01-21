using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PeerTutoringNetwork.Controllers;
using PeerTutoringNetwork.DesignPatterns;
using PeerTutoringNetwork.Viewmodels;
using Xunit;

namespace Matko.Tests
{
    public class SubjectsControllerTests
    {
        private readonly Mock<IRepository<Subject>> _subjectRepositoryMock;
        private readonly Mock<IFactory<Subject, SubjectVM>> _subjectFactoryMock;
        private readonly Mock<IUtils> _utilsMock;
        private readonly SubjectsController _controller;

        public SubjectsControllerTests()
        {
            _subjectRepositoryMock = new Mock<IRepository<Subject>>();
            _subjectFactoryMock = new Mock<IFactory<Subject, SubjectVM>>();
            _utilsMock = new Mock<IUtils>();
            _controller = new SubjectsController(
                _subjectRepositoryMock.Object,
                _subjectFactoryMock.Object,
                _utilsMock.Object);
            SeedMocks();
        }

        private void SeedMocks()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 101,
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
                    UserId = 102,
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
                new Subject { SubjectId = 201, SubjectName = "Mathematics", Description = "Advanced Mathematics", CreatedByUserId = 101 },
                new Subject { SubjectId = 202, SubjectName = "Physics", Description = "Physics Fundamentals", CreatedByUserId = 102 }
            };

            _utilsMock.Setup(utils => utils.GetUsersAsync()).ReturnsAsync(users);
            _subjectRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(subjects);
            _subjectRepositoryMock.Setup(repo => repo.GetByIdAsync(201)).ReturnsAsync(subjects.First());
            _subjectRepositoryMock.Setup(repo => repo.GetByIdAsync(202)).ReturnsAsync(subjects.Last());
            _subjectFactoryMock.Setup(factory => factory.CreateVM(It.IsAny<Subject>()))
                .Returns<Subject>(s => new SubjectVM
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName,
                    Description = s.Description,
                    CreatedByUserId = s.CreatedByUserId
                });
            _subjectFactoryMock.Setup(factory => factory.CreateModel(It.IsAny<SubjectVM>()))
                .Returns<SubjectVM>(vm => new Subject
                {
                    SubjectId = vm.SubjectId,
                    SubjectName = vm.SubjectName,
                    Description = vm.Description,
                    CreatedByUserId = vm.CreatedByUserId
                });
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
            _subjectRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Subject>()), Times.Once);
        }

        [Fact]
        public async Task Create_InvalidModel_ReturnsViewResult()
        {
            // Arrange
            _controller.ModelState.AddModelError("SubjectName", "Required");
            var subjectVM = new SubjectVM
            {
                Description = "Life Science",
                CreatedByUserId = 101
            };

            // Act
            var result = await _controller.Create(subjectVM);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(subjectVM, viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_ValidModel_UpdatesSubject()
        {
            // Arrange
            var subjectVM = new SubjectVM
            {
                SubjectId = 201,
                SubjectName = "Updated Math",
                Description = "Updated Mathematics",
                CreatedByUserId = 101
            };

            // Act
            var result = await _controller.Edit(subjectVM.SubjectId, subjectVM);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            _subjectRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Subject>()), Times.Once);
        }

        [Fact]
        public async Task Edit_InvalidModel_ReturnsViewResult()
        {
            // Arrange
            _controller.ModelState.AddModelError("SubjectName", "Required");
            var subjectVM = new SubjectVM
            {
                SubjectId = 201,
                Description = "Updated Mathematics",
                CreatedByUserId = 101
            };

            // Act
            var result = await _controller.Edit(subjectVM.SubjectId, subjectVM);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(subjectVM, viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_ForInvalidId()
        {
            // Arrange
            var invalidId = 999; // ID that does not exist in the repository
            var subjectVM = new SubjectVM
            {
                SubjectId = invalidId,
                SubjectName = "Nonexistent Subject",
                Description = "This subject does not exist",
                CreatedByUserId = 101
            };

            // Act
            var result = await _controller.Edit(invalidId, subjectVM);

            // Assert
            Assert.IsType<NotFoundResult>(result); // Ensure it returns a NotFoundResult
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
        }

        [Fact]
        public async Task Details_ReturnsNotFound_ForInvalidId()
        {
            // Arrange
            var invalidId = 999;

            // Act
            var result = await _controller.Details(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesSubject()
        {
            // Act
            var result = await _controller.DeleteConfirmed(201);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            _subjectRepositoryMock.Verify(repo => repo.DeleteAsync(201), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsSubjectDetails()
        {
            // Act
            var result = await _controller.Delete(201);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<SubjectVM>(viewResult.ViewData.Model);
            Assert.Equal(201, model.SubjectId);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_ForInvalidId()
        {
            // Arrange
            var invalidId = 999;

            // Act
            var result = await _controller.Delete(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
