using Microsoft.AspNetCore.SignalR;
using SocialAppLibrary.Shared.Dtos;

namespace SocialApp.Api.ServiceInterface.ChatService
{
    public interface IChatService
    {
        public interface IChatService
        {
            Task<ApiResult<bool>> SendMessageToUserAsync(Guid fromUserId, Guid toUserId, string message);
            Task<ApiResult<bool>> SendMessageToAllAsync(string message);
            Task AddConnectionAsync(Guid userId, string connectionId);
            Task RemoveConnectionAsync(Guid userId);
        }
    }
}