using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialApp.Api.Data.Entities
{
    public class Follow
    {
        [Key]
        public Guid Id { get; set; } // Khóa chính
        public Guid FollowerId { get; set; } // Người theo dõi
        public Guid FollowingId { get; set; } // Người được theo dõi
        public DateTime FollowedOn { get; set; } // Thời gian theo dõi
        public bool IsActive { get; set; } // Trạng thái (đang theo dõi hay đã hủy)

        [ForeignKey("FollowerId")]
        public virtual User Follower { get; set; }

        [ForeignKey("FollowingId")]
        public virtual User Following { get; set; }
    }
}