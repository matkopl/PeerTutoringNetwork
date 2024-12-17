using System;
using System.Collections.Generic;

namespace BL.Models;

public partial class Subject
{
    public int SubjectId { get; set; }

    public string SubjectName { get; set; }

    public string? Description { get; set; }

    public int CreatedByUserId { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual User CreatedByUser { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
