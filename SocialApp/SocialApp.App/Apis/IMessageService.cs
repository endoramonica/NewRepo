using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.Dtos.ChatDto;

namespace SocialApp.App.Apis
{
    public interface IMessageService
    {
        Task<ApiResult<int>> AddMessageAsync(Guid fromUserId, Guid toUserId, string message);
        Task<ApiResult<IEnumerable<LastestMessageDto>>> GetLastestMessageAsync(Guid userId);
        Task<ApiResult<IEnumerable<MessageDto>>> GetMessagesAsync(Guid fromUserId, Guid toUserId);
    }


}
