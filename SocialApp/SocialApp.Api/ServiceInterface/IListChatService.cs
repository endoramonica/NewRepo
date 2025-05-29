namespace SocialApp.Api.ServiceInterface
{
    public interface IListChatService
    {
        Task<ListChatInitializeResponseDto> InitializeAsync(Guid userId);
    }
}
