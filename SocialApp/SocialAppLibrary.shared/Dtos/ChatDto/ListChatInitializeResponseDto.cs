namespace SocialAppLibrary.Shared.Dtos.ChatDto
{
    public class ListChatInitializeResponseDto
    {
        public UserDto User { get; set; } = null!;
        public IEnumerable<UserDto> UserFriends { get; set; } = null!;
        public IEnumerable<LastestMessageDto> LastestMessages { get; set; } = null!;
    }
}