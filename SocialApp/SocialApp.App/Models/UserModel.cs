using CommunityToolkit.Mvvm.ComponentModel;
using SocialAppLibrary.Shared.Dtos;
using System;

namespace SocialApp.App.Models
{
    public partial class UserModel : ObservableObject
    {
        public Guid ID { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhotoUrl { get; set; }

        /// <summary>
        /// Hiển thị tên, nếu không có sẽ fallback về Email.
        /// </summary>
        public string DisplayName => string.IsNullOrWhiteSpace(Name) ? Email : Name;

        /// <summary>
        /// Hiển thị ảnh đại diện, nếu không có sẽ dùng ảnh mặc định.
        /// </summary>
        public string DisplayPhotoUrl => string.IsNullOrWhiteSpace(PhotoUrl) ? "add_a_photo.png" : PhotoUrl;

        // --- Trạng thái UI ---

        [ObservableProperty]
        private bool isFollowing;   // Người dùng hiện tại có đang theo dõi người này không?

        [ObservableProperty]
        private bool isFollower;    // Người này có đang theo dõi lại người dùng hiện tại không?

        [ObservableProperty]
        private bool isBusy;        // Dùng để disable UI khi đang xử lý theo dõi, huỷ theo dõi...

        [ObservableProperty]
        private bool isSelected;    // Ví dụ: dùng trong UI để chọn nhiều người



        public static UserModel FromLoggedInUser(LoggedInUser user)
        {
            return new UserModel
            {
                ID = user.ID,
                Name = user.Name,
                Email = user.Email,
                PhotoUrl = user.PhotoUrl
            };

        }
    }
}