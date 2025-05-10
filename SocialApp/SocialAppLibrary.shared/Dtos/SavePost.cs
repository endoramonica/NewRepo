using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace SocialAppLibrary.Shared.Dtos
{
    public class SavePost
    {
        public Guid PostId { get; set; }
        public string? Content { get; set; }
        [JsonIgnore] // Tránh serialize khi trả về JSON
        public IFormFile? Photo { get; set; }
        public bool IsExistingPhotoRemoved { get; set; }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Content) && Photo is null)
            {
                return false;
            }return true;
        }
    }
}
