using Microsoft.AspNetCore.Mvc;
using PeerTutoringNetwork.Viewmodels;
using BL.Models;
using PeerTutoringNetwork.DesignPatterns;

namespace PeerTutoringNetwork.Controllers
{
    public class MentorDashboardController : Controller
    {
        private readonly PeerTutoringNetworkContext _context;
        private readonly DashboardFacade _dashboardFacade;

        public MentorDashboardController(PeerTutoringNetworkContext context, DashboardFacade dashboardFacade)
        {
            _context = context;
            _dashboardFacade = dashboardFacade;
        }

        public IActionResult Index()
        {
            var dashboardData = _dashboardFacade.GetMentorDashboardData();

            return View(dashboardData);
        }
    }
}
