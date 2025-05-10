using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.Marshalling;
namespace SocialApp.Api.Data.Entities
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        public string? Content { get; set; }

        [Comment("Physical path of image")]
        public string? PhotoPath { get; set; }

        public string? PhotoUrl { get; set; }

        public DateTime PostedOn { get; set; }
        public DateTime? NotificationOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? ModifiedOn { get; internal set; }
        //public string? PhotoHash { get; set; }
        //[Timestamp] // Đánh dấu để EF theo dõi xung đột
       // public byte[] RowVersion { get; set; }
    }
}
