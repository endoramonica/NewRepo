using Microsoft.AspNetCore.Http;
using Refit;
using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.Dtos.ChatDto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialApp.App.Apis;

[Headers("Authorization: Bearer")]
public interface IUserApi
{
    [Multipart]
    [Post("/api/user/change-photo")]
    Task<ApiResult<string>> ChangePhotoAsync(StreamPart photo);

    [Get("/api/user/posts")]
    Task<PostDto[]> GetUserPostsAsync(int startIndex, int pageSize);

    [Get("/api/user/bookmarked-posts")]
    Task<PostDto[]> GetUserBookmarkedPostsAsync(int startIndex, int pageSize);

    [Get("/api/user/liked-posts")]
    Task<PostDto[]> GetUserLikedPostsAsync(int startIndex, int pageSize);

    [Get("/api/user/notifications")]
    Task<NotificationDto[]> GetNotificationsAsync(int startIndex, int pageSize);

    [Get("/api/user/friends/{userId}")]
    Task<ApiResult<IEnumerable<UserDto>>> GetUserFriendsAsync(Guid userId);

    [Post("/api/user/friends/request")]
    Task<ApiResult<bool>> SendFriendRequestAsync([Body] FriendRequestDto dto);

    [Put("/api/user/friends/accept")]
    Task<ApiResult<bool>> AcceptFriendRequestAsync([Body] FriendActionDto dto);

    [Put("/api/user/friends/reject")]
    Task<ApiResult<bool>> RejectFriendRequestAsync([Body] FriendActionDto dto);

    [Get("/api/user/friends/pending/{userId}")]
    Task<ApiResult<IEnumerable<UserDto>>> GetPendingFriendRequestsAsync(Guid userId);

    [Delete("/api/user/friends/remove")]
    Task<ApiResult<bool>> RemoveFriendAsync([Body] FriendActionDto dto);
}
