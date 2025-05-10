using System.ComponentModel.DataAnnotations;

namespace SocialAppLibrary.Shared.Dtos
{
    public class SaveCommentDto
    {
        public Guid PostId { get; set; }
        public Guid CommentId { get; set; }

        [Required]
        public string? Content { get; set; }

        public bool Validate()
        {
            return !string.IsNullOrWhiteSpace(Content);
        }
    }
}
