public class MessageDto
{
    public int Id { get; set; }
    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime SendDateTime { get; set; }
    public bool IsRead { get; set; }
}
