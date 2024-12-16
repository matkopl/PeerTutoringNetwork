namespace PeerTutoringNetwork.Viewmodels
{
    public class AppointmentVM
    {
        public int MentorId { get; set; }
        public string MentorUsername { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
