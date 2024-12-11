using System;
using System.Collections.Generic;

namespace BL.Models;

public partial class Message
{
    public int Id { get; set; }

    public int? ChatId { get; set; }

    public int? SenderId { get; set; }

    public string? Content { get; set; }

    public DateTime? SentAt { get; set; }

    public virtual Chat? Chat { get; set; }

    public virtual User? Sender { get; set; }
}
