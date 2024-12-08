using System;
using System.Collections.Generic;

namespace PeerTutoringNetwork.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int MentorId { get; set; }

    public int SubjectId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public virtual ICollection<AppointmentReservation> AppointmentReservations { get; set; } = new List<AppointmentReservation>();

    public virtual User Mentor { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
