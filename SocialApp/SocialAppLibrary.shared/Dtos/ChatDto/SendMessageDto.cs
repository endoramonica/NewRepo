public class SendMessageDto
{
    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }
    public string Content { get; set; } = null!;
}
