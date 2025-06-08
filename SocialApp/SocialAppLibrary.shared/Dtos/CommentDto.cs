using System;
using System.Collections.ObjectModel;

namespace SocialAppLibrary.Shared.Dtos
{
    public class CommentDto
    {
        public Guid PostId { get; set; }
        public Guid CommentId { get; set; }
        public Guid? ParentCommentId { get; set; } // Xác định bình luận cha (cho trả lời)
        public string? Content { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserPhotoUrl { get; set; }
        public DateTime AddedOn { get; set; }
        public bool IsLiked { get; set; } // Trạng thái người dùng hiện tại đã thích
        public int LikeCount { get; set; } // Số lượt thích
        public ObservableCollection<CommentDto> Replies { get; set; } = []; // Danh sách bình luận trả lời
        public string CommentedOnDisplay => AddedOn.ToString("dd MMM yyyy | HH:mm");
        public string DisplayPhotoUrl => string.IsNullOrWhiteSpace(UserPhotoUrl) ? "add_a_photo.png" : UserPhotoUrl;
    }
}