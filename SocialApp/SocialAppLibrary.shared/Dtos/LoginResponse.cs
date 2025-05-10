namespace SocialAppLibrary.Shared.Dtos
{
    public record LoginResponse(LoggedInUser User, string Token);
}
