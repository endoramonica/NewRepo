using Microsoft.AspNetCore.Http;
using Refit;
using SocialAppLibrary.Shared.Dtos;

namespace SocialApp.App.Apis;

[Headers("Authorization: Bearer ")]
public interface IUserApi
{
    [Post("/api/user/change-photo")]
    Task<ApiResult<string>> ChangePhotoAsync(IFormFile photo);
    [Get("/api/user/posts")]
    Task<PostDto[]> GetUserPostsAsync(int startIndex, int pageSize);
    [Get("/api/user/bookmarked-posts")]
    Task<PostDto[]> GetUserBookmarkedPostsAsync(int startIndex, int pageSize);
    [Get("/api/user/liked-posts")]
    Task<PostDto[]> GetUserLikedPostsAsync(int startIndex, int pageSize);
}
