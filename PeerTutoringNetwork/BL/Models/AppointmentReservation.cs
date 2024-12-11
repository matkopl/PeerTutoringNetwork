using System;
using System.Collections.Generic;

namespace BL.Models;

public partial class AppointmentReservation
{
    public int ReservationId { get; set; }

    public int AppointmentId { get; set; }

    public int StudentId { get; set; }

    public DateTime? ReservationTime { get; set; }

    public virtual Appointment Appointment { get; set; } 

    public virtual User Student { get; set; } 
}
