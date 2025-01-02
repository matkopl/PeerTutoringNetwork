namespace PeerTutoringNetwork.Viewmodels
{
    public class ReservationVM
    {
        public int ReservationId { get; set; } 
        public string StudentName { get; set; } = string.Empty; 
        public DateTime ReservationTime { get; set; } 
        public int AppointmentId { get; set; }
        public string MentorUsername { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string AppointmentDetails { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public bool IsReservation { get; set; }
    }
}
