namespace PeerTutoringNetwork.Viewmodels
{
    public class ReservationVM
    {
        public int ReservationId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public DateTime ReservationTime { get; set; }
        public string AppointmentDetails { get; set; } = string.Empty;
    }
}
