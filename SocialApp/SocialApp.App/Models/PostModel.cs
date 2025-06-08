using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SocialAppLibrary.Shared.Dtos;
using SocialApp.App.Apis;
using SocialApp.App.Services;

namespace SocialApp.App.Models
{
    public partial class PostModel : ObservableObject
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }

        [ObservableProperty, NotifyPropertyChangedFor(nameof(UserPhoto))]
        private string? _userPhotoUrl;
        public string UserPhoto => string.IsNullOrWhiteSpace(UserPhotoUrl) ? "add_a_photo.png" : UserPhotoUrl;

        public string? Content { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime PostedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public string PostedOnDisplay =>
            ModifiedOn?.ToString("dd MMM yy HH:mm") ??
            PostedOn.ToString("dd MMM yy HH:mm");

        public string PostTemplateContentViewName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PhotoUrl))
                {
                    return "WithNoImage";
                }
                if (string.IsNullOrWhiteSpace(Content))
                {
                    return "OnlyImage";
                }
                return "WithImage";
            }
        }

        [ObservableProperty, NotifyPropertyChangedFor(nameof(IsLikedIcon))]
        private bool _isLiked;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(IsBookmarkedIcon))]
        private bool _isBookmarked;

        // Thêm properties cho Follow
        [ObservableProperty, NotifyPropertyChangedFor(nameof(FollowButtonText), nameof(ShowFollowButton))]
        private bool _isFollowing;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(ShowFollowButton))]
        private bool _isCurrentUser;

        public string IsLikedIcon => IsLiked ? "heart_f.png" : "heart.png";
        public string IsBookmarkedIcon => IsBookmarked ? "bookmark_heart_f.png" : "ngoisao.png";

        // Logic cho Follow Button
        public string FollowButtonText => IsFollowing ? "Following" : "Follow";
        public bool ShowFollowButton => !IsCurrentUser;

        public static PostModel FromDto(PostDto dto)
        {
            return new PostModel
            {
                PostId = dto.PostId,
                UserId = dto.UserId,
                UserName = dto.UserName,
                UserPhotoUrl = dto.UserPhotoUrl,
                Content = dto.Content,
                PhotoUrl = dto.PhotoUrl,
                PostedOn = dto.PostedOn,
                ModifiedOn = dto.ModifiedOn,
                IsLiked = dto.IsLiked,
                IsBookmarked = dto.IsBookmarked
            };
        }
        public async Task InitializeAsync(IFollowApi followApi, AuthService authService)
        {
            try
            {
                IsCurrentUser = UserId == authService.User.ID;
                var isFollowingResult = await followApi.IsFollowingAsync(UserId);
                IsFollowing = isFollowingResult.IsSuccess ? isFollowingResult.Data : false;
            }
            catch (Exception ex)
            {
                // Log lỗi và đặt giá trị mặc định  
                IsFollowing = false;
            }
        }
    }
}