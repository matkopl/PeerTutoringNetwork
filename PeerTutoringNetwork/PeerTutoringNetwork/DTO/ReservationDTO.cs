using System;

namespace PeerTutoringNetwork.DTOs
{
    public class ReservationDTO
    {
        public int ReservationId { get; set; } 
        public int AppointmentId { get; set; } 
        public int StudentId { get; set; } 
        public string StudentName { get; set; } 
        public DateTime AppointmentDate { get; set; } 
    }
}
