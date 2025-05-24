using Microsoft.AspNetCore.Http;
using Refit;
using SocialAppLibrary.Shared.Dtos;

namespace SocialApp.App.Apis;

[Headers("Authorization: Bearer ")]
public interface IPostApi
{
    //[Multipart]
    //[Post("/api/posts/save")]
    //Task<ApiResult> SavePostAsync(StreamPart? photo, string serializedSavePostDto);

    //[Multipart]
    //[Post("/api/posts/save")]
    //Task<ApiResult> SavePostAsync(
    //     [AliasAs("photo")] IFormFile? photo, // Không cần [FromForm] vì Refit tự hiểu trong Multipart
    //     [AliasAs("SerializedSavePostDto")] string serializedSavePostDto
    // );
    [Multipart]
    [Post("/api/posts/save")]
    Task<ApiResult<PostDto>> SavePostAsync([AliasAs("photo")] StreamPart? photo, [AliasAs("SerializedSavePostDto")] string? SerializedSavePostDto);

    [Get("/api/posts")]
    Task<PostDto[]> GetPostsAsync(int startIndex, int pageSize);
    [Get("/api/posts/{postId}")]
    Task<PostDto?> GetPostAsync(Guid postId);
    [Post("/api/posts/{postId}/comments")]
    Task<ApiResult<CommentDto>> SaveCommentAsync(Guid postId, SaveCommentDto dto);

    [Get("/api/posts/{postId}/comments")]
    Task<CommentDto[]> GetCommentsAsync(Guid postId,   int startIndex,  int pageSize);
    [Post("/api/posts/{postId}/toggle-like")]
    Task<ApiResult> ToggleLikeAsync(Guid postId);
    [Post("/api/posts/{postId}/toggle-bookmark")]
    Task<ApiResult> ToggleBookmarkAsync(Guid postId);
    [Delete("/api/posts/{postId}/DeletePost")]
    Task<ApiResult> DeletePostAsync(Guid postId);
}
