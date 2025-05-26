using Refit;
using SocialAppLibrary.Shared.Dtos;

namespace SocialApp.App.Apis;
[Headers("Authorization: Bearer ")]
public interface IFollowApi
    {    [Post("/api/follow")]
    Task<ApiResult<string>> FollowAsync([Query] Guid followingId);

    [Delete("/api/follow")]
    Task<ApiResult<string>> UnfollowAsync([Query] Guid followingId);

    [Get("/api/follow/followers")]
    Task<ApiResult<LoggedInUser[]>> GetFollowersAsync(int startIndex, int pageSize);

    [Get("/api/follow/following")]
    Task<ApiResult<LoggedInUser[]>> GetFollowingAsync(int startIndex, int pageSize);

    [Get("/api/follow/followers/search")]
    Task<ApiResult<LoggedInUser[]>> SearchFollowersAsync([Query] string q, int startIndex, int pageSize);
}
