using Moq;
using Xunit;
using BL.Repositories;
using BL.Models;
using BL.lnterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Tests.AntonioTest.Repo
{
    public class ReviewRepositoryTests
    {
        private readonly Mock<PeerTutoringNetworkContext> _contextMock;
        private readonly Mock<DbSet<Review>> _dbSetMock;
        private readonly ReviewRepository _reviewRepository;

        public ReviewRepositoryTests()
        {
            // Mock the DbSet<Review>
            _dbSetMock = new Mock<DbSet<Review>>();

            // Mock the context
            _contextMock = new Mock<PeerTutoringNetworkContext>();
            _contextMock.Setup(c => c.Reviews).Returns(_dbSetMock.Object);

            // Instantiate the ReviewRepository with the mocked context
            _reviewRepository = new ReviewRepository(_contextMock.Object);
        }

        [Fact]
        public void AddReview_ShouldCallSaveChanges()
        {
            // Arrange
            var review = new Review { ReviewId = 1, UserId = 1, SubjectId = 1, Rating = 5, Comment = "Great" };

            // Act
            _reviewRepository.AddReview(review);

            // Assert
            _contextMock.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateReview_ShouldCallSaveChanges()
        {
            // Arrange
            var review = new Review { ReviewId = 1, UserId = 1, SubjectId = 1, Rating = 5, Comment = "Great" };

            // Act
            _reviewRepository.UpdateReview(review);

            // Assert
            _contextMock.Verify(c => c.SaveChanges(), Times.Once);
        }
    }
}
