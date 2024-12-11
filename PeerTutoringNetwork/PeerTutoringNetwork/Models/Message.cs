using System;
using System.Collections.Generic;

namespace PeerTutoringNetwork.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int ChatId { get; set; }

    public int SenderId { get; set; }

    public string? Content { get; set; }

    public DateTime? SentAt { get; set; }

    public virtual Chat Chat { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
