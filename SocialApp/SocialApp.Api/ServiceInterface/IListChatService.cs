namespace SocialApp.Api.ServiceInterface
{
    public interface IListChatService
    {
        Task<ListChatInitializeResponseDto> InitializeAsync(int userId);
    }
}
