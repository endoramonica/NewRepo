//namespace SocialAppLibrary.Shared.Dtos
//{
//    public record NotificationDto(Guid ForUserId, string Text, DateTime When, Guid? PostId);
//    public string CommentedOnDisplay => AddedOn.ToString("dd MMM yyyy | HH:mm");
//    public string DisplayPhotoUrl => string.IsNullOrWhiteSpace(UserPhotoUrl) ? "add_a_photo.png" : UserPhotoUrl;
//}
namespace SocialAppLibrary.Shared.Dtos
{
    public record NotificationDto(
        Guid ForUserId,
        string Text,
        DateTime When,
        Guid? PostId,
        string? UserPhotoUrl // 🟡 Bạn cần thêm trường này nếu muốn xử lý ảnh
    )
    {
        public string CommentedOnDisplay => When.ToString("dd MMM yyyy | HH:mm");

        public string DisplayPhotoUrl =>
            string.IsNullOrWhiteSpace(UserPhotoUrl)
                ? "add_a_photo.png"
                : UserPhotoUrl;
    }
}
