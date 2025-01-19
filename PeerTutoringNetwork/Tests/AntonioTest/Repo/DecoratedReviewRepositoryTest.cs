using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using BL.Models;
using BL.Repositories;
using BL.lnterfaces;

namespace Tests.AntonioTest.Repo
{
    public class DecoratedReviewRepositoryTests
    {
        private readonly Mock<IReviewRepository> _innerRepositoryMock;
        private readonly DecoratedReviewRepository _decoratedRepository;

        public DecoratedReviewRepositoryTests()
        {
           
            _innerRepositoryMock = new Mock<IReviewRepository>();

       
            _decoratedRepository = new DecoratedReviewRepository(_innerRepositoryMock.Object);
        }

        [Fact]
        public void AddReview_ShouldLogWhenCalled()
        {
            // Arrange
            var review = new Review
            {
                ReviewId = 1,
                UserId = 1,
                SubjectId = 1,
                Rating = 5,
                Comment = "Excellent!"
            };

       
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                // Act
                _decoratedRepository.AddReview(review);

                // Assert
                var consoleOutput = sw.ToString();
                Assert.Contains($"[LOG] Adding review for User ID: {review.UserId}, Subject ID: {review.SubjectId}", consoleOutput);
            }
        }

        [Fact]
        public void GetReviews_ShouldLogWhenCalled()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review { ReviewId = 1, UserId = 1, SubjectId = 1, Rating = 5, Comment = "Excellent!" },
                new Review { ReviewId = 2, UserId = 2, SubjectId = 2, Rating = 4, Comment = "Good." }
            };

            _innerRepositoryMock.Setup(repo => repo.GetReviews()).Returns(reviews);


            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                // Act
                _decoratedRepository.GetReviews();

                // Assert
                var consoleOutput = sw.ToString();
                Assert.Contains("[LOG] Fetching all reviews.", consoleOutput);
            }
        }
    }
}
