using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.Dtos.ChatDto;

namespace SocialApp.Api.ServiceInterface
{
    public interface IUserFriendService
    {
        Task<ApiResult<IEnumerable<UserDto>>> GetUserFriendsAsync(Guid userId);
        Task<ApiResult<bool>> SendFriendRequestAsync(Guid fromUserId, Guid toUserId);
        Task<ApiResult<bool>> AcceptFriendRequestAsync(Guid userId, Guid friendId);
        Task<ApiResult<bool>> RejectFriendRequestAsync(Guid userId, Guid friendId);
        Task<ApiResult<IEnumerable<UserDto>>> GetPendingFriendRequestsAsync(Guid userId);
        Task<ApiResult<bool>> RemoveFriendAsync(Guid userId, Guid friendId);
    }
}
