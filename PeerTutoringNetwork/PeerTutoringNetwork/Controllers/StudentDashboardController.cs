using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PeerTutoringNetwork.Viewmodels;
using BL.Models;
using PeerTutoringNetwork.DTO;

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

        //private int? GetCurrentUserId()
        //{
        //    try
        //    {
        //        var token = HttpContext.Request.Cookies["jwtToken"];
        //
        //        var handler = new JwtSecurityTokenHandler();
        //        if (!handler.CanReadToken(token))
        //        {
        //            _logger.LogWarning("Unable to read JWT token.");
        //            return null;
        //        }
        //
        //        var jwtToken = handler.ReadJwtToken(token);
        //
        //        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId");
        //        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        //        {
        //            return userId;
        //        }
        //
        //        _logger.LogWarning("UserId claim not found in the token.");
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error decoding JWT token: {ex.Message}");
        //        return null;
        //    }
        //}

        [HttpGet("[action]/{userId}")]
        public ActionResult GetUserById(int userId)
        {
            try
            {
                // Dohvati korisnika prema userId
                var user = _context.Users
                    .Where(u => u.UserId == userId)
                    .Select(u => new UserProfileDto
                    {
                        UserId = u.UserId,
                        Username = u.Username,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Phone = u.Phone,
                        RoleId = u.RoleId
                    })
                    .FirstOrDefault();

                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }




        public async Task<IActionResult> Index()
        {
           var userId = _context.Users.Select(u => u.UserId).FirstOrDefault();
            // Fetch student reservations
            var reservations = await _context.AppointmentReservations
                .Include(r => r.Appointment)
                .ThenInclude(a => a.Mentor)
                .Include(r => r.Appointment.Subject)
                .Where(r => r.StudentId == userId)
                .Select(r => new ReservationVM
                {
                    ReservationId = r.ReservationId,
                    StudentName = r.Student.Username,
                    ReservationTime = r.ReservationTime ?? DateTime.Now,
                    AppointmentDetails = $"{r.Appointment.Subject.SubjectName} - {r.Appointment.Mentor.Username} at {r.Appointment.AppointmentDate:yyyy-MM-dd HH:mm}"
                })
                .ToListAsync();

            // Fetch available appointments
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

            // Combine data into a single view model
            var dashboardVM = new StudentDashboardVM
            {
                Reservations = reservations,
                AvailableAppointments = availableAppointments
            };

            return View(dashboardVM);
        }
    }
}
