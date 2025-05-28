using SocialAppLibrary.Shared.Dtos;

namespace SocialApp.Api.ServiceInterface
{
    public interface IMessageService
    {
        Task<ApiResult<int>> AddMessageAsync(Guid fromUserId, Guid toUserId, string message);
        Task<ApiResult<IEnumerable<LastestMessageDto>>> GetLastestMessageAsync(Guid userId);
        Task<ApiResult<IEnumerable<MessageDto>>> GetMessagesAsync(Guid fromUserId, Guid toUserId);
    }


}
