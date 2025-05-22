namespace SocialAppLibrary.Shared.Dtos
{
    public record LoggedInUser(Guid ID, string Name, string Email, string? PhotoUrl)
    {
        public string DisplayName => string.IsNullOrWhiteSpace(Name) ? Email : Name;
        public string DisplayPhotoUrl => string.IsNullOrWhiteSpace(PhotoUrl) ? "add_a_photo.png" : PhotoUrl;
    }

}
