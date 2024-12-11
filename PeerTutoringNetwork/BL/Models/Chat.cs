﻿using System;
using System.Collections.Generic;

namespace BL.Models;

public partial class Chat
{
    public int ChatId { get; set; }

    public string? Title { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
