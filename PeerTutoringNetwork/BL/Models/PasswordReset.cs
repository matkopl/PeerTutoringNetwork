using System;
using System.Collections.Generic;

namespace BL.Models;

public partial class PasswordReset
{
    public int ResetId { get; set; }

    public int UserId { get; set; }

    public string ResetToken { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public virtual User User { get; set; } = null!;
}
