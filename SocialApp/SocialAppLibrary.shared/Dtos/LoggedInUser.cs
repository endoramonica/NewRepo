namespace SocialAppLibrary.Shared.Dtos
{
    public record LoggedInUser(Guid ID, string Name, string Email, string? PhotoUrl);
}
