using Microsoft.EntityFrameworkCore;
using SocialApp.Api.Data;
using SocialApp.Api.Data.Entities;
using SocialApp.Api.ServiceInterface;
using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.Dtos.ChatDto;


namespace SocialApp.Api.Services
{
    public class MessageService : IMessageService
    {
        private readonly DataContext _dataContext;
        private readonly AuthService _authService;

        public MessageService(DataContext dataContext, AuthService authService)
        {
            _dataContext = dataContext;
            _authService = authService;
        }

        public async Task<ApiResult<int>> AddMessageAsync(Guid fromUserId, Guid toUserId, string message)
        {
            try
            {
                var entity = new Message
                {
                    FromUserId = fromUserId,
                    ToUserId = toUserId,
                    Content = message,
                    SendDateTime = DateTime.UtcNow,
                    IsRead = false
                };

                _dataContext.Message.Add(entity);

                var result = await _dataContext.SaveChangesAsync();

                return ApiResult<int>.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResult<int>.Fail(ex.Message);
            }
        }

        public async Task<ApiResult<IEnumerable<LastestMessageDto>>> GetLastestMessageAsync(Guid userId)
        {
            try
            {
                var result = new List<LastestMessageDto>();
                var userFriends = await _dataContext.UserFriends
                    .Where(x => x.UserId == userId)
                    .ToListAsync();

                foreach (var userFriend in userFriends)
                {
                    var lastMessage = await _dataContext.Message
                        .Where(x => (x.FromUserId == userId && x.ToUserId == userFriend.FriendId)
                                 || (x.FromUserId == userFriend.FriendId && x.ToUserId == userId))
                        .OrderByDescending(x => x.SendDateTime)
                        .FirstOrDefaultAsync();

                    if (lastMessage != null)
                    {
                        var friendInfo = await _authService.GetUserByIdAsync(userFriend.FriendId);
                        if (friendInfo.IsSuccess)
                        {
                            result.Add(new LastestMessageDto
                            {
                                UserId = userId,
                                Content = lastMessage.Content,
                                UserFriendInfo = friendInfo.Data!,
                                Id = lastMessage.Id,
                                IsRead = lastMessage.IsRead,
                                SendDateTime = lastMessage.SendDateTime
                            });
                        }
                    }
                }
                return ApiResult<IEnumerable<LastestMessageDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<LastestMessageDto>>.Fail(ex.Message);
            }
        }

        public async Task<ApiResult<IEnumerable<MessageDto>>> GetMessagesAsync(Guid fromUserId, Guid toUserId)
        {
            try
            {
                var entities = await _dataContext.Message
                    .Where(x => (x.FromUserId == fromUserId && x.ToUserId == toUserId)
                             || (x.FromUserId == toUserId && x.ToUserId == fromUserId))
                    .OrderBy(x => x.SendDateTime)
                    .ToListAsync();

                var dtos = entities.Select(x => new MessageDto
                {
                    Id = x.Id,
                    Content = x.Content,
                    FromUserId = x.FromUserId,
                    ToUserId = x.ToUserId,
                    SendDateTime = x.SendDateTime,
                    IsRead = x.IsRead
                });

                return ApiResult<IEnumerable<MessageDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<MessageDto>>.Fail(ex.Message);
            }
        }
    }
}