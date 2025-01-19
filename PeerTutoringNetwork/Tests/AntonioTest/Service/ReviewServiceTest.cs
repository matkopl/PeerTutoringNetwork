using Moq;
using Xunit;
using BL.Models;
using BL.Services;
using BL.lnterfaces;
using System.Collections.Generic;
using System.Linq;

namespace Tests.AntonioTest.Service
{
    public class ReviewServiceTests
    {
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly ReviewService _reviewService;

        public ReviewServiceTests()
        {
            // Initialize the mock repository
            _reviewRepositoryMock = new Mock<IReviewRepository>();

            // Initialize the service with the mocked repository
            _reviewService = new ReviewService(_reviewRepositoryMock.Object);
        }

        [Fact]
        public void GetReviewDetails_ShouldReturnReview_WhenReviewExists()
        {
            // Arrange
            var reviewId = 1;
            var expectedReview = new Review { ReviewId = reviewId, Rating = 5, Comment = "Great subject!" };

            _reviewRepositoryMock.Setup(r => r.GetReviewById(reviewId))
                .Returns(expectedReview);

            // Act
            var result = _reviewService.GetReviewDetails(reviewId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedReview.ReviewId, result.ReviewId);
            Assert.Equal(expectedReview.Rating, result.Rating);
            Assert.Equal(expectedReview.Comment, result.Comment);

            // Verify that the repository method was called once
            _reviewRepositoryMock.Verify(r => r.GetReviewById(reviewId), Times.Once);
        }

        [Fact]
        public void GetAllReviews_ShouldReturnAllReviews()
        {
            // Arrange
            var allReviews = new List<Review>
            {
                new Review { ReviewId = 1, Rating = 5, Comment = "Great subject!" },
                new Review { ReviewId = 2, Rating = 4, Comment = "Good subject." }
            };

            _reviewRepositoryMock.Setup(r => r.GetAllReviews())
                .Returns(allReviews);

            // Act
            var result = _reviewService.GetAllReviews();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(allReviews.Count, result.Count());
            Assert.Equal(allReviews[0].ReviewId, result.First().ReviewId);
            Assert.Equal(allReviews[1].ReviewId, result.Last().ReviewId);

            // Verify that the repository method was called once
            _reviewRepositoryMock.Verify(r => r.GetAllReviews(), Times.Once);
        }

        [Fact]
        public void GetReviewsBySubjectId_ShouldReturnReviewsForSubject()
        {
            // Arrange
            var subjectId = 1;
            var reviewsForSubject = new List<Review>
            {
                new Review { ReviewId = 1, Rating = 5, Comment = "Great subject!" },
                new Review { ReviewId = 2, Rating = 4, Comment = "Good subject." }
            };

            _reviewRepositoryMock.Setup(r => r.GetReviewsBySubjectId(subjectId))
                .Returns(reviewsForSubject);

            // Act
            var result = _reviewService.GetReviewsBySubjectId(subjectId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reviewsForSubject.Count, result.Count());
            Assert.Equal(reviewsForSubject[0].ReviewId, result.First().ReviewId);
            Assert.Equal(reviewsForSubject[1].ReviewId, result.Last().ReviewId);

            // Verify that the repository method was called once
            _reviewRepositoryMock.Verify(r => r.GetReviewsBySubjectId(subjectId), Times.Once);
        }

        [Fact]
        public void GetReviewsByUserId_ShouldReturnReviewsForUser()
        {
            // Arrange
            var userId = 1;
            var reviewsForUser = new List<Review>
            {
                new Review { ReviewId = 1, Rating = 5, Comment = "Great subject!" },
                new Review { ReviewId = 2, Rating = 4, Comment = "Good subject." }
            };

            _reviewRepositoryMock.Setup(r => r.GetReviewsByUserId(userId))
                .Returns(reviewsForUser);

            // Act
            var result = _reviewService.GetReviewsByUserId(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reviewsForUser.Count, result.Count());
            Assert.Equal(reviewsForUser[0].ReviewId, result.First().ReviewId);
            Assert.Equal(reviewsForUser[1].ReviewId, result.Last().ReviewId);

            // Verify that the repository method was called once
            _reviewRepositoryMock.Verify(r => r.GetReviewsByUserId(userId), Times.Once);
        }

        [Fact]
        public void CreateReview_ShouldCallAddReview_WhenReviewIsValid()
        {
            // Arrange
            var newReview = new Review { ReviewId = 1, Rating = 5, Comment = "Amazing!" };

            // Act
            _reviewService.CreateReview(newReview);

            // Assert
            _reviewRepositoryMock.Verify(r => r.AddReview(newReview), Times.Once);
        }

        [Fact]
        public void EditReview_ShouldCallUpdateReview_WhenReviewIsValid()
        {
            // Arrange
            var updatedReview = new Review { ReviewId = 1, Rating = 4, Comment = "Updated review!" };

            // Act
            _reviewService.EditReview(updatedReview);

            // Assert
            _reviewRepositoryMock.Verify(r => r.UpdateReview(updatedReview), Times.Once);
        }

        [Fact]
        public void RemoveReview_ShouldCallDeleteReview_WhenReviewExists()
        {
            // Arrange
            var reviewId = 1;

            // Act
            _reviewService.RemoveReview(reviewId);

            // Assert
            _reviewRepositoryMock.Verify(r => r.DeleteReview(reviewId), Times.Once);
        }
    }
}
