using Microsoft.EntityFrameworkCore;
using SocialApp.Api.Data;
using SocialApp.Api.Data.Entities;
using SocialApp.Api.ServiceInterface;
using SocialAppLibrary.Shared.Dtos;
using Microsoft.AspNetCore.SignalR;
using SocialApp.Api.Hubs;
using SocialAppLibrary.Shared.IHub;
using SocialAppLibrary.Shared.Dtos.ChatDto;

namespace SocialApp.Api.Services
{
    public class UserFriendService : IUserFriendService
    {
        private readonly DataContext _dataContext;
        private readonly AuthService _authService;
        private readonly IHubContext<SocialHubs, ISocialHubClient> _hubContext;

        public UserFriendService(DataContext dataContext, AuthService authService, IHubContext<SocialHubs, ISocialHubClient> hubContext)
        {
            _dataContext = dataContext;
            _authService = authService;
            _hubContext = hubContext;
        }

        public async Task<ApiResult<IEnumerable<UserDto>>> GetUserFriendsAsync(Guid userId)
        {
            try
            {
                var friendEntities = await _dataContext.UserFriends
                    .Where(x => x.UserId == userId && x.Status == 1)
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

        public async Task<ApiResult<bool>> SendFriendRequestAsync(Guid fromUserId, Guid toUserId)
        {
            try
            {
                if (fromUserId == toUserId)
                    return ApiResult<bool>.Fail("Cannot send friend request to yourself");

                var fromUser = await _dataContext.Users.FindAsync(fromUserId);
                var toUser = await _dataContext.Users.FindAsync(toUserId);
                if (fromUser == null || toUser == null)
                    return ApiResult<bool>.Fail("User not found");

                var existingRequest = await _dataContext.UserFriends
                    .AnyAsync(x => x.UserId == fromUserId && x.FriendId == toUserId);
                if (existingRequest)
                    return ApiResult<bool>.Fail("Friend request already sent or users are already friends");

                var friendRequest = new UserFriend
                {
                    UserId = fromUserId,
                    FriendId = toUserId,
                    Status = 0, // Pending
                    CreatedAt = DateTime.UtcNow
                };
                _dataContext.UserFriends.Add(friendRequest);

                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    ForUserId = toUserId,
                    When = DateTime.UtcNow,
                    Text = $"{fromUser.Name} sent you a friend request",
                    PostId = null
                };
                _dataContext.Notifications.Add(notification);

                await _dataContext.SaveChangesAsync();

                // Gửi thông báo thời gian thực
                await _hubContext.Clients.User(toUserId.ToString()).FriendRequestSent(
                    new FriendRequestNotificationDto(fromUserId, fromUser.Name, toUserId));

                return ApiResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<ApiResult<bool>> AcceptFriendRequestAsync(Guid userId, Guid friendId)
        {
            try
            {
                var request = await _dataContext.UserFriends
                    .FirstOrDefaultAsync(x => x.UserId == friendId && x.FriendId == userId && x.Status == 0);
                if (request == null)
                    return ApiResult<bool>.Fail("Friend request not found");

                request.Status = 1; // Accepted
                _dataContext.UserFriends.Update(request);

                var reverseRequest = new UserFriend
                {
                    UserId = userId,
                    FriendId = friendId,
                    Status = 1, // Accepted
                    CreatedAt = DateTime.UtcNow
                };
                _dataContext.UserFriends.Add(reverseRequest);

                var fromUser = await _dataContext.Users.FindAsync(userId);
                var toUser = await _dataContext.Users.FindAsync(friendId);
                if (fromUser != null && toUser != null)
                {
                    var notification = new Notification
                    {
                        Id = Guid.NewGuid(),
                        ForUserId = friendId,
                        When = DateTime.UtcNow,
                        Text = $"{fromUser.Name} accepted your friend request",
                        PostId = null
                    };
                    _dataContext.Notifications.Add(notification);

                    await _dataContext.SaveChangesAsync();

                    // Gửi thông báo thời gian thực
                    await _hubContext.Clients.User(friendId.ToString()).FriendRequestAccepted(
                        new FriendRequestNotificationDto(userId, fromUser.Name, friendId));
                }

                return ApiResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<ApiResult<bool>> RejectFriendRequestAsync(Guid userId, Guid friendId)
        {
            try
            {
                var request = await _dataContext.UserFriends
                    .FirstOrDefaultAsync(x => x.UserId == friendId && x.FriendId == userId && x.Status == 0);
                if (request == null)
                    return ApiResult<bool>.Fail("Friend request not found");

                request.Status = 2; // Rejected
                _dataContext.UserFriends.Update(request);

                var fromUser = await _dataContext.Users.FindAsync(userId);
                var toUser = await _dataContext.Users.FindAsync(friendId);
                if (fromUser != null && toUser != null)
                {
                    var notification = new Notification
                    {
                        Id = Guid.NewGuid(),
                        ForUserId = friendId,
                        When = DateTime.UtcNow,
                        Text = $"{fromUser.Name} rejected your friend request",
                        PostId = null
                    };
                    _dataContext.Notifications.Add(notification);

                    await _dataContext.SaveChangesAsync();

                    // Gửi thông báo thời gian thực
                    await _hubContext.Clients.User(friendId.ToString()).FriendRequestRejected(
                        new FriendRequestNotificationDto(userId, fromUser.Name, friendId));
                }

                return ApiResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<ApiResult<IEnumerable<UserDto>>> GetPendingFriendRequestsAsync(Guid userId)
        {
            try
            {
                var pendingRequests = await _dataContext.UserFriends
                    .Where(x => x.FriendId == userId && x.Status == 0)
                    .ToListAsync();

                var pendingDtos = new List<UserDto>();
                foreach (var request in pendingRequests)
                {
                    var userInfo = await _authService.GetUserByIdAsync(request.UserId);
                    if (userInfo.IsSuccess && userInfo.Data != null)
                    {
                        pendingDtos.Add(userInfo.Data);
                    }
                }

                return ApiResult<IEnumerable<UserDto>>.Success(pendingDtos);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<UserDto>>.Fail(ex.Message);
            }
        }

        public async Task<ApiResult<bool>> RemoveFriendAsync(Guid userId, Guid friendId)
        {
            try
            {
                var friendship1 = await _dataContext.UserFriends
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.FriendId == friendId && x.Status == 1);
                var friendship2 = await _dataContext.UserFriends
                    .FirstOrDefaultAsync(x => x.UserId == friendId && x.FriendId == userId && x.Status == 1);

                if (friendship1 == null && friendship2 == null)
                    return ApiResult<bool>.Fail("Friendship not found");

                if (friendship1 != null)
                    _dataContext.UserFriends.Remove(friendship1);
                if (friendship2 != null)
                    _dataContext.UserFriends.Remove(friendship2);

                var fromUser = await _dataContext.Users.FindAsync(userId);
                var toUser = await _dataContext.Users.FindAsync(friendId);
                if (fromUser != null && toUser != null)
                {
                    var notification = new Notification
                    {
                        Id = Guid.NewGuid(),
                        ForUserId = friendId,
                        When = DateTime.UtcNow,
                        Text = $"{fromUser.Name} has removed you from their friends list",
                        PostId = null
                    };
                    _dataContext.Notifications.Add(notification);

                    await _dataContext.SaveChangesAsync();

                    // Gửi thông báo thời gian thực
                    await _hubContext.Clients.User(friendId.ToString()).FriendRemoved(
                        new FriendRequestNotificationDto(userId, fromUser.Name, friendId));
                }

                return ApiResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }
    }
}