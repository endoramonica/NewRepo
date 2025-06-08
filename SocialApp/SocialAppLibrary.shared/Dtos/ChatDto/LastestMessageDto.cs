namespace SocialAppLibrary.Shared.Dtos.ChatDto
{
    public class LastestMessageDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public UserDto UserFriendInfo { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime SendDateTime { get; set; }
        public bool IsRead { get; set; }
    }
}