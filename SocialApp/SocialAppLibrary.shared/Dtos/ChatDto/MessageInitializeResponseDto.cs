namespace SocialAppLibrary.Shared.Dtos.ChatDto
{
    public class MessageInitializeResponseDto
    {
        public UserDto FriendInfo { get; set; } = null!;
        public IEnumerable<MessageDto> Messages { get; set; } = null!;
    }
}
   