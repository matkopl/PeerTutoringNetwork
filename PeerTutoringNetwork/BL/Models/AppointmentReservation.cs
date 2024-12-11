using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BL.Models;

public partial class AppointmentReservation
{
    public int ReservationId { get; set; }

    [Required]
    public int AppointmentId { get; set; }

    [Required]
    public int StudentId { get; set; }

    public DateTime? ReservationTime { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual User Student { get; set; } = null!;
}
