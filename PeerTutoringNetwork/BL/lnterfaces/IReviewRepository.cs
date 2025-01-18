using BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.lnterfaces
{// 3. Repository pattern -- ovo je Repository pattern jer se koristi za pristupanje podacima
    public interface IReviewRepository
    {
        Review GetReviewById(int reviewId);
        IEnumerable<Review> GetAllReviews();
        IEnumerable<Review> GetReviewsBySubjectId(int subjectId);
        IEnumerable<Review> GetReviewsByUserId(int userId);
        void AddReview(Review review);
        List<Review> GetReviews();
        void UpdateReview(Review review);
        void DeleteReview(int reviewId);
    }
}
