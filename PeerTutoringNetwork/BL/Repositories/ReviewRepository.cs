using BL.lnterfaces;
using BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Repositories
{ // 3. Repository pattern -- ovo je Repository pattern jer je klasa koja se koristi za pristupanje podacima
    public class ReviewRepository : IReviewRepository
    {
        private readonly PeerTutoringNetworkContext _context;

        public ReviewRepository(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        public Review GetReviewById(int reviewId)
        {
            return _context.Reviews.FirstOrDefault(r => r.ReviewId == reviewId);
        }

        public IEnumerable<Review> GetAllReviews()
        {
            return _context.Reviews.ToList();
        }

        public IEnumerable<Review> GetReviewsBySubjectId(int subjectId)
        {
            return _context.Reviews.Where(r => r.SubjectId == subjectId).ToList();
        }

        public IEnumerable<Review> GetReviewsByUserId(int userId)
        {
            return _context.Reviews.Where(r => r.UserId == userId).ToList();
        }

        public void AddReview(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }

        public void UpdateReview(Review review)
        {
            _context.Reviews.Update(review);
            _context.SaveChanges();
        }

        public void DeleteReview(int reviewId)
        {
            var review = GetReviewById(reviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                _context.SaveChanges();
            }
        }

        List<Review> IReviewRepository.GetReviews()
        {
            return _context.Reviews.ToList();
        }
    }
}
