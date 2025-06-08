using SocialAppLibrary.Shared.Dtos.ChatDto;

namespace SocialApp.Api.ServiceInterface
{
    public interface IListChatService
    {
        Task<ListChatInitializeResponseDto> InitializeAsync(Guid userId);
    }
}
