using SocialAppLibrary.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialAppLibrary.Shared.IHub
{
     public interface ISocialHubClient
    {
        Task PostChange(PostDto postDto);

        Task PostDelete(Guid postId);
        Task PostLike(Guid postId, Guid userId);
        Task PostUnLike(Guid postId, Guid userId);
        Task PostBookmark(Guid postId, Guid userId);
        Task PostUnBookmark(Guid postId, Guid userId);
        Task PostCommentAdded(CommentDto commentDto);
        Task UserPhotoChange( UserPhotoChange userPhotoChange );
        Task NotificationGenerated(NotificationDto notificationDto);
    }

    public record struct UserPhotoChange(Guid UserId, string? UserPhotoUrl);
}
