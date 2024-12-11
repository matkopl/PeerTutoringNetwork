using Microsoft.AspNetCore.Mvc;
using PeerTutoringNetwork.Viewmodels;
using BL.Models;

namespace PeerTutoringNetwork.Controllers
{
    public class MentorDashboardController : Controller
    {
        private readonly PeerTutoringNetworkContext _context;

        public MentorDashboardController(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var dashboardData = new MentorDashboardVM
            {
                TotalReservations = _context.AppointmentReservations.Count(),
                TotalSubjects = _context.Subjects.Count(),
                RecentReservations = _context.AppointmentReservations
                    .OrderByDescending(r => r.ReservationTime)
                    .Take(5)
                    .ToList(),
                RecentSubjects = _context.Subjects
                    .OrderByDescending(s => s.SubjectId)
                    .Take(5)
                    .ToList()
            };

            return View(dashboardData);
        }
    }
}
