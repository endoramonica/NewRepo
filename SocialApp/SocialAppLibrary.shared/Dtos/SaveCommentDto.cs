using System;
using System.ComponentModel.DataAnnotations;

namespace SocialAppLibrary.Shared.Dtos
{
    public class SaveCommentDto
    {
        public Guid PostId { get; set; }
        public Guid CommentId { get; set; }
        public Guid? ParentCommentId { get; set; } // Xác định bình luận cha (cho trả lời)

        [Required]
        public string? Content { get; set; }

        public bool Validate()
        {
            return !string.IsNullOrWhiteSpace(Content);
        }
    }
}