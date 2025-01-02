namespace PeerTutoringNetwork.Viewmodels
{
    public class StudentDashboardVM
    {
        public List<AppointmentVM> AvailableAppointments { get; set; } = new List<AppointmentVM>();
        public List<ReservationVM> Reservations { get; set; } = new List<ReservationVM>();
    }
}
