using System;
using System.Collections.Generic;

namespace PeerTutoringNetwork.Models;

public partial class Profile
{
    public int ProfileId { get; set; }

    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Bio { get; set; }

    public virtual User User { get; set; } = null!;
}
