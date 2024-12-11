using BL.Models;

namespace PeerTutoringNetwork.Viewmodels
{
    public class MentorDashboardVM
    {
        public int TotalReservations { get; set; }
        public int TotalSubjects { get; set; }
        public IList<AppointmentReservation> RecentReservations { get; set; }
        public IList<Subject> RecentSubjects { get; set; }
    }
}
