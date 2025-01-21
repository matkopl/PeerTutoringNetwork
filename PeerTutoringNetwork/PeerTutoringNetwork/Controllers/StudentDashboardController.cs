using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using PeerTutoringNetwork.DesignPatterns;

namespace PeerTutoringNetwork.Controllers
{
    public class StudentDashboardController : Controller
    {
        private readonly DashboardFacade _dashboardFacade;

        public StudentDashboardController(DashboardFacade dashboardFacade)
        {
            _dashboardFacade = dashboardFacade;
        }

        public async Task<IActionResult> Index()
        {
            var token = Request.Query["jwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                return Redirect("/Login.html");
            }

            var userId = ExtractUserIdFromToken(token);

            if (userId == null)
            {
                return Unauthorized();
            }

            var dashboardData = await _dashboardFacade.GetStudentDashboardData(userId.Value);
            return View(dashboardData);
        }

        private int? ExtractUserIdFromToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
                {
                    return userId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding token: {ex.Message}");
            }
            return null;
        }
    }
}
