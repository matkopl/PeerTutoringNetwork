namespace PeerTutoringNetwork.DTO
{
    public class SubjectDto
    {
        public int SubjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UserId { get; set; } // Administrator koji kreira subject
    }
}
