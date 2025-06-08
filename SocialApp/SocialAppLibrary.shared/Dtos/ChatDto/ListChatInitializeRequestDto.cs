using SocialAppLibrary.Shared.Dtos.ChatDto;

public class ListChatInitializeRequestDto
{
    public UserDto User { get; set; } = null!;
    public IEnumerable<UserDto> Friends { get; set; } = null!;
    public IEnumerable<LastestMessageDto> Messages { get; set; } = null!;
}