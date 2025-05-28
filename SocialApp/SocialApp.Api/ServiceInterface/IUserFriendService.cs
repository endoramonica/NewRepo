using SocialAppLibrary.Shared.Dtos;

namespace SocialApp.Api.ServiceInterface
{
    public interface IUserFriendService
    {
        Task<ApiResult<IEnumerable<UserDto>>> GetUserFriendsAsync(Guid userId);
    }
}
