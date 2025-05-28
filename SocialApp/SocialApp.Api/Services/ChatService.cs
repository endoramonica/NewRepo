using Microsoft.AspNetCore.SignalR;
using SocialApp.Api.Hubs;
using SocialApp.Api.ServiceInterface;
using SocialApp.Api.ServiceInterface.ChatService;
using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.IHub;

namespace SocialApp.Api.Services
{
    public class ChatService : IChatService
    {
        private static readonly Dictionary<Guid, List<string>> _connectionMapping = new Dictionary<Guid, List<string>>();
        private readonly IMessageService _messageService;
        private readonly IHubContext<SocialHubs, ISocialHubClient> _hubContext;

        public ChatService(IMessageService messageService, IHubContext<SocialHubs, ISocialHubClient> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
        }

        public async Task<ApiResult<bool>> SendMessageToUserAsync(Guid fromUserId, Guid toUserId, string message)
        {
            try
            {
                var connectionIds = _connectionMapping
                    .Where(x => x.Key == toUserId)
                    .SelectMany(x => x.Value)
                    .ToList();

                var result = await _messageService.AddMessageAsync(fromUserId, toUserId, message);
                if (!result.IsSuccess)
                    return ApiResult<bool>.Fail(result.Error);

                if (!connectionIds.Any())
                    return ApiResult<bool>.Fail("Recipient is not connected");

                await _hubContext.Clients.Clients(connectionIds)
                    .ReceiveMessage(fromUserId, message);

                return ApiResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<ApiResult<bool>> SendMessageToAllAsync(string message)
        {
            try
            {
                await _hubContext.Clients.All.ReceiveMessage(Guid.Empty, message);
                return ApiResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }

        public async Task AddConnectionAsync(Guid userId, string connectionId)
        {
            await Task.Run(() =>
            {
                lock (_connectionMapping)
                {
                    if (!_connectionMapping.ContainsKey(userId))
                    {
                        _connectionMapping[userId] = new List<string>();
                    }
                    if (!_connectionMapping[userId].Contains(connectionId))
                    {
                        _connectionMapping[userId].Add(connectionId);
                    }
                }
            });
        }

        public async Task RemoveConnectionAsync(Guid userId)
        {
            await Task.Run(() =>
            {
                lock (_connectionMapping)
                {
                    _connectionMapping.Remove(userId);
                }
            });
        }
    }
}