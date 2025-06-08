
using Refit;
using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.Dtos.ChatDto;

namespace SocialApp.App.Apis
{
    public interface IListChatService
    {
        [Post("/api/message/initialize")]
        Task<ApiResult<MessageInitializeResponseDto>> InitializeAsync([Body] MessageInitializeRequestDto request);

    }
}
