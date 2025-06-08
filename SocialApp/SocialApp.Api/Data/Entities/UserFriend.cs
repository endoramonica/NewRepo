namespace SocialApp.Api.Data.Entities
{
    public class UserFriend
    {
        public Guid UserId { get; set; }
        public Guid FriendId { get; set; }
        public int Status { get; set; } // 0: pending, 1: accepted, 2: rejected
        public DateTime CreatedAt { get; set; } // Thời điểm gửi lời mời
        public User User { get; set; } = null!;
        public User Friend { get; set; } = null!;
    }
}