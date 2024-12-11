using System;
using System.Collections.Generic;

namespace BL.Models;

public partial class Session
{
    public int SessionId { get; set; }

    public int UserId { get; set; }

    public DateTime? LoginTime { get; set; }

    public DateTime? LogoutTime { get; set; }

    public virtual User User { get; set; } = null!;
}
