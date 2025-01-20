namespace PeerTutoringNetwork.Viewmodels;

public class MessageVM
{
    public int MessageId { get; set; }

    public int ChatId { get; set; }

    public int SenderId { get; set; }

    public string? Content { get; set; }

    public DateTime? SentAt { get; set; }
}