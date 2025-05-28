namespace SocialApp.Api.Data.Entities
{
    public class UserFriend
    {
        public Guid UserId { get; set; }
        public Guid FriendId { get; set; }

        public User User { get; set; } = null!;
        public User Friend { get; set; } = null!;
    }
}