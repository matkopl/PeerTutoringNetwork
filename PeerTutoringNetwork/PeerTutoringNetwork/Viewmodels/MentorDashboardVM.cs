using BL.Models;

namespace PeerTutoringNetwork.Viewmodels
{
    public class MentorDashboardVM
    {
        public int TotalAppointments { get; set; }
        public int TotalSubjects { get; set; }
        public IList<Appointment> RecentAppointments { get; set; }
        public IList<Subject> RecentSubjects { get; set; }
    }
}
