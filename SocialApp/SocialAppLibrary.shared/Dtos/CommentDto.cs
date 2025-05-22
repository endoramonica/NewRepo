namespace SocialAppLibrary.Shared.Dtos
{
    public class CommentDto
    {
        public Guid PostId { get; set; }
        public Guid CommentId { get; set; }
        public string? Content { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserPhotoUrl { get; set; }
        public DateTime AddedOn { get; set; }
        public string CommentedOnDisplay => AddedOn.ToString("dd MMM yyyy | HH:mm");
        public string DisplayPhotoUrl => string.IsNullOrWhiteSpace(UserPhotoUrl) ? "add_a_photo.png" : UserPhotoUrl;
    }
}
