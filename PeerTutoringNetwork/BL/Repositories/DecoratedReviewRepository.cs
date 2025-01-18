using BL.lnterfaces;
using BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Repositories
{/* 4. Decorator pattern -- ovo je Decorator pattern jer se koristi
  za dodavanje funkcionalnosti na postojeći objekt to se može videti
  u klasi DecoratedReviewRepository zbog metoda AddReview i GetReviews
  koje dodaju funkcionalnosti na postojeći objekat*/
    public class DecoratedReviewRepository
    {
        private readonly IReviewRepository _innerRepository;

        public DecoratedReviewRepository(IReviewRepository innerRepository)
        {
            _innerRepository = innerRepository;
        }

        public void AddReview(Review review)
        {
            Console.WriteLine($"[LOG] Adding review for User ID: {review.UserId}, Subject ID: {review.SubjectId}");
            _innerRepository.AddReview(review);
        }

        public List<Review> GetReviews()
        {
            Console.WriteLine("[LOG] Fetching all reviews.");
            return _innerRepository.GetReviews();
        }
    }
}
