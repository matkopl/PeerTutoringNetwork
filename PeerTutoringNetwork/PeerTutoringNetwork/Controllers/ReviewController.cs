using BL.Models;
using BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace PeerTutoringNetwork.Controllers
{// 3. Repository pattern -- jer u ovom kontroleru se koristi ReviewService koji koristi IReviewRepository
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("{id}")]
        public IActionResult GetReview(int id)
        {
            var review = _reviewService.GetReviewDetails(id);
            if (review == null)
                return NotFound("Review not found");

            return Ok(review);
        }

        [HttpGet]
        public IActionResult GetAllReviews()
        {
            return Ok(_reviewService.GetAllReviews());
        }

        [HttpGet("subject/{subjectId}")]
        public IActionResult GetReviewsBySubject(int subjectId)
        {
            return Ok(_reviewService.GetReviewsBySubjectId(subjectId));
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetReviewsByUser(int userId)
        {
            return Ok(_reviewService.GetReviewsByUserId(userId));
        }

        [HttpPost]
        public IActionResult CreateReview([FromBody] Review review)
        {
            _reviewService.CreateReview(review);
            return Ok("Review created successfully");
        }

        [HttpPut("{id}")]
        public IActionResult EditReview(int id, [FromBody] Review review)
        {
            if (id != review.ReviewId)
                return BadRequest("Review ID mismatch");

            _reviewService.EditReview(review);
            return Ok("Review updated successfully");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteReview(int id)
        {
            _reviewService.RemoveReview(id);
            return Ok("Review deleted successfully");
        }
    }
}
