using BL.lnterfaces;
using BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{ // 3. Repository pattern -- jer u ovom slučaju klasa ReviewService koristi ReviewRepository za pristupanje podacima
    public class ReviewService
    {
        private readonly IReviewRepository _reviewRepository; 

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public Review GetReviewDetails(int reviewId)
        {
            return _reviewRepository.GetReviewById(reviewId);
        }

        public IEnumerable<Review> GetAllReviews()
        {
            return _reviewRepository.GetAllReviews();
        }

        public IEnumerable<Review> GetReviewsBySubjectId(int subjectId)
        {
            return _reviewRepository.GetReviewsBySubjectId(subjectId);
        }

        public IEnumerable<Review> GetReviewsByUserId(int userId)
        {
            return _reviewRepository.GetReviewsByUserId(userId);
        }

        public void CreateReview(Review review)
        {
            _reviewRepository.AddReview(review);
        }

        public void EditReview(Review review)
        {
            _reviewRepository.UpdateReview(review);
        }

        public void RemoveReview(int reviewId)
        {
            _reviewRepository.DeleteReview(reviewId);
        }
    }
}
