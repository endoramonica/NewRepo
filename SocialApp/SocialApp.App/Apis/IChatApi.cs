using Refit;
using SocialAppLibrary.Shared.Dtos;

namespace SocialApp.App.Apis;

public interface IChatApi
{
    [Post("/api/chat/send")]
    Task<ApiResult<bool>> SendMessageAsync([Body] SendMessageDto dto);

    [Post("/api/chat/broadcast")]
    Task<ApiResult<bool>> BroadcastMessageAsync([Body] BroadcastMessageRequest dto);

    [Post("/api/chat/connect")]
    Task<ApiResult<bool>> AddConnectionAsync([Body] ConnectionRequest dto);

    [Post("/api/chat/disconnect")]
    Task<ApiResult<bool>> RemoveConnectionAsync([Body] DisconnectionRequest dto);
}
