using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PeerTutoringNetwork.Viewmodels;
using BL.Models;
using System.IdentityModel.Tokens.Jwt;

namespace PeerTutoringNetwork.Controllers
{
    public class StudentDashboardController : Controller
    {
        private readonly PeerTutoringNetworkContext _context;
        private readonly ILogger<StudentDashboardController> _logger;

        public StudentDashboardController(PeerTutoringNetworkContext context, ILogger<StudentDashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var token = Request.Query["jwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token is missing in the request.");
                return Redirect("/Login.html");
            }

            var userId = ExtractUserIdFromToken(token); // Implement your JWT decoding here

            if (userId == null)
            {
                _logger.LogWarning("Invalid token.");
                return Unauthorized();
            }

            var reservations = await _context.AppointmentReservations
                .Include(r => r.Appointment)
                .ThenInclude(a => a.Mentor)
                .Include(r => r.Appointment.Subject)
                .Where(r => r.StudentId == userId)
                .Select(r => new ReservationVM
                {
                    ReservationId = r.ReservationId,
                    AppointmentId = r.AppointmentId,
                    MentorUsername = r.Appointment.Mentor.Username,
                    SubjectName = r.Appointment.Subject.SubjectName,
                    ReservationTime = r.ReservationTime ?? DateTime.Now
                })
                .ToListAsync();

            var availableAppointments = await _context.Appointments
                .Include(a => a.Mentor)
                .Include(a => a.Subject)
                .Where(a => !_context.AppointmentReservations.Any(r => r.AppointmentId == a.AppointmentId))
                .Select(a => new AppointmentVM
                {
                    AppointmentId = a.AppointmentId,
                    MentorId = a.MentorId,
                    SubjectId = a.SubjectId,
                    MentorUsername = a.Mentor.Username,
                    SubjectName = a.Subject.SubjectName,
                    AppointmentDate = a.AppointmentDate
                })
                .ToListAsync();

            var dashboardVM = new StudentDashboardVM
            {
                Reservations = reservations,
                AvailableAppointments = availableAppointments
            };

            return View(dashboardVM);
        }

        private int? ExtractUserIdFromToken(string token)
        {
            // Decode the JWT token to extract userId
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
                _logger.LogError($"Failed to decode token: {ex.Message}");
            }
            return null;
        }
    }
}
