//using SocialApp.Api.ServiceInterface;

//namespace SocialApp.Api.Services
//{
//    public class ListChatService : IListChatService
//    {
        
//        private readonly UserService _userService;
//        private readonly UserFriendService _userFriendService;
//        private readonly MessageService _messageService;

//        public ListChatService(UserService userService , UserFriendService userFriendService , MessageService messageService)
//        {
//            _userService = userService;
//            _userFriendService = userFriendService;
//            _messageService = messageService;
//        }

//        public async Task<ListChatInitializeResponseDto> InitializeAsync(int userId)
//        {
//            try
//            {
//                if (userId <= 0)
//                {
//                    throw new ArgumentException("Invalid user ID.");
//                }

//                var response = new ListChatInitializeResponseDto
//                {
//                    User = _userService.GetUserById(userId),
//                    UserFriends = await _userFriendService.GetUserFriendsAsync(userId),
//                    LastestMessages = await _userService.GetLastestMessage(userId)
//                };

//                if (response.User == null)
//                {
//                    throw new InvalidOperationException("User not found.");
//                }

//                return response;
//            }
//            catch (Exception ex)
//            {
//                throw new InvalidOperationException($"Failed to initialize chat data for user {userId}: {ex.Message}", ex);
//            }
//        }
//    }
//}