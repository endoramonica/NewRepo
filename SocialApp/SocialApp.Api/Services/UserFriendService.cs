using Microsoft.EntityFrameworkCore;
using SocialApp.Api.Data;
using SocialApp.Api.ServiceInterface;
using SocialAppLibrary.Shared.Dtos;

namespace SocialApp.Api.Services
{
    public class UserFriendService : IUserFriendService
    {
        private readonly DataContext _dataContext;
        private readonly AuthService _authService;

        public UserFriendService(DataContext dataContext, AuthService authService)
        {
            _dataContext = dataContext;
            _authService = authService;
        }

        public async Task<ApiResult<IEnumerable<UserDto>>> GetUserFriendsAsync(Guid userId)
        {
            try
            {
                var friendEntities = await _dataContext.UserFriends
                    .Where(x => x.UserId == userId)
                    .ToListAsync();

                var friendDtos = new List<UserDto>();
                foreach (var friend in friendEntities)
                {
                    var friendInfo = await _authService.GetUserByIdAsync(friend.FriendId);
                    if (friendInfo.IsSuccess && friendInfo.Data != null)
                    {
                        friendDtos.Add(friendInfo.Data);
                    }
                }

                return ApiResult<IEnumerable<UserDto>>.Success(friendDtos);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<UserDto>>.Fail(ex.Message);
            }
        }
    }
}