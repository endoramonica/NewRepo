using System.Diagnostics;
using System.Text.Json.Serialization;

namespace SocialAppLibrary.Shared.Dtos
{
    public class PostDto
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserPhotoUrl { get; set; }
        public string? Content { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime PostedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    
        [JsonIgnore]
        public string PostedOnDisplay =>
    ModifiedOn?.ToString("dd MMM yy HH:mm") ??
    PostedOn.ToString("dd MMM yy HH:mm");

        public bool IsLiked { get; set; }
        public bool IsBookmarked { get; set; }

        
    }
}
