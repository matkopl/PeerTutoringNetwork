using System;
using System.Collections.Generic;

namespace PeerTutoringNetwork.Models;

public partial class LoginAttempt
{
    public int AttemptId { get; set; }

    public int UserId { get; set; }

    public DateTime Timestamp { get; set; }

    public bool Successful { get; set; }

    public virtual User User { get; set; } = null!;
}
