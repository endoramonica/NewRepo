using SocialApp.Api.ServiceInterface;
using SocialAppLibrary.Shared.Dtos.ChatDto;

namespace SocialApp.Api.Services
{
    public class ListChatService :IListChatService
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly IUserFriendService _userFriendService;
        private readonly IMessageService _messageService;

        public ListChatService(AuthService authService, UserService userService, IUserFriendService userFriendService, IMessageService messageService)
        {
            _authService = authService;
            _userService = userService;
            _userFriendService = userFriendService;
            _messageService = messageService;
        }

        public async Task<ListChatInitializeResponseDto> InitializeAsync(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("Invalid user ID.");
                }

                var userResult = await _authService.GetUserByIdAsync(userId);
                if (!userResult.IsSuccess || userResult.Data == null)
                {
                    throw new InvalidOperationException($"User not found: {userResult}");
                }

                var userFriendsResult = await _userFriendService.GetUserFriendsAsync(userId);
                var lastMessagesResult = await _messageService.GetLastestMessageAsync(userId);

                return new ListChatInitializeResponseDto
                {
                    User = userResult.Data,
                    UserFriends = userFriendsResult.Data,
                    LastestMessages = lastMessagesResult.Data
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize chat data for user {userId}: {ex.Message}", ex);
            }
        }


    }
}
