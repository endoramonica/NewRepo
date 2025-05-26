namespace SocialAppLibrary.Shared.Dtos;

public record FollowNotificationDto(
    Guid FollowerId,      // Người follow
    Guid FollowingId,     // Người được follow - THÊM FIELD NÀY
    string Message,
    LoggedInUser Follower
);
