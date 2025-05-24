using Microsoft.AspNetCore.Mvc;
using SocialApp.Api.Services;
using SocialAppLibrary.Shared.Dtos;
using System.Security.Claims;
using System.Text.Json;
using System.IO; // Thêm namespace này nếu dùng Stream


//#22
namespace SocialApp.Api.Endpoint
{
    public static class PostsEndpoints
    {
        public static IEndpointRouteBuilder MapPostsEndpoints(this IEndpointRouteBuilder app)
        {
            // Tạo nhóm endpoint với tiền tố "/api/posts", yêu cầu xác thực và gắn tag "Posts" cho Swagger
            var postsGroup = app.MapGroup("/api/posts")
                .RequireAuthorization()
                .WithTags("Posts");

            ////Endpoint để lưu bài viết mới hoặc cập nhật bài viết(POST: / api / posts / save)
            //postsGroup.MapPost("/save", async( IFormFile ? photo, string SerializedSavePostDto,  PostService postService, ClaimsPrincipal principal) =>
            //{
            //    if (string.IsNullOrWhiteSpace(SerializedSavePostDto))
            //        return Results.BadRequest("Missing data");

            //    SavePost dto = JsonSerializer.Deserialize<SavePost>(SerializedSavePostDto)!;

            //    dto.Photo = photo;

            //    return Results.Ok(await postService.SavePostAsync(dto, principal.GetUserId()));
            //})
            //.DisableAntiforgery()
            //.Produces<ApiResult>(StatusCodes.Status200OK)
            //.Produces<ApiResult>(StatusCodes.Status400BadRequest)
            //.WithName("SavePost")
            //.Accepts<SavePost>("multipart/form-data"); // Thêm để hỗ trợ multipart

            //            postsGroup.MapPost("/save", async ([FromForm] IFormFile? photo, [FromForm] string SerializedSavePostDto, PostService postService, ClaimsPrincipal principal) =>
            //            {
            //                if (string.IsNullOrWhiteSpace(SerializedSavePostDto))
            //                    return Results.BadRequest("Missing data");
            //                SavePost dto = JsonSerializer.Deserialize<SavePost>(SerializedSavePostDto)!;
            //                dto.Photo = photo;
            //                return Results.Ok(await postService.SavePostAsync(dto, principal.GetUserId()));
            //            })
            //.DisableAntiforgery().WithName("SavePost")
            //.Accepts<SavePost>("multipart/form-data");
            postsGroup.MapPost("/save", async (
[FromForm] SavePostRequest request,
PostService postService,
ClaimsPrincipal principal) =>
            {
                if (string.IsNullOrWhiteSpace(request.SerializedSavePostDto))
                    return Results.BadRequest(ApiResult.Fail("Missing data string in serialized save post"));

                SavePost dto;
                try
                {
                    dto = JsonSerializer.Deserialize<SavePost>(request.SerializedSavePostDto)!;
                    if (dto == null)
                        return Results.BadRequest(ApiResult.Fail("Invalid save post data"));
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ApiResult.Fail($"Failed to deserialize save post data: {ex.Message}"));
                }

                dto.Photo = request.Photo;
                return Results.Ok(await postService.SavePostAsync(dto, principal.GetUser()));
            })
.Produces<ApiResult<PostDto>>(StatusCodes.Status200OK)
.Produces<ApiResult<PostDto>>(StatusCodes.Status400BadRequest)
.DisableAntiforgery()
.WithName("SavePost");

            // Endpoint để lấy danh sách bài viết với phân trang (GET: /api/posts/)
            postsGroup.MapGet("/", async (int startIndex, int pageSize, PostService postService, ClaimsPrincipal principal) =>
                Results.Ok(await postService.GetPostsAsync(startIndex, pageSize, principal.GetUserId())))
                .Produces<PostDto[]>(StatusCodes.Status200OK)
                .WithName("GetPosts");
            // Endpoint để lấy danh sách bài viết  với postid (GET: /api/posts/)
            postsGroup.MapGet("/{postId:guid}", async (Guid postId, PostService postService, ClaimsPrincipal principal) =>
                Results.Ok(await postService.GetPostAsync(postId, principal.GetUserId())))
                .Produces<PostDto>(StatusCodes.Status200OK)
                .WithName("GetPostsById");

            // Endpoint để lưu hoặc cập nhật bình luận cho bài viết (POST: /api/posts/{postId}/comments)
            postsGroup.MapPost("/{postId:guid}/comments", async (Guid postId, SaveCommentDto dto, PostService postService, ClaimsPrincipal principal) =>
            {
                dto.PostId = postId; // Gán postId từ URL vào DTO
                var result = await postService.SaveCommentAsync(dto, principal.GetUser());
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
                .Produces<ApiResult<CommentDto>>(StatusCodes.Status200OK)
                .Produces<ApiResult<CommentDto>>(StatusCodes.Status400BadRequest)
                .WithName("SaveComment");

            // Endpoint để lấy danh sách bình luận của bài viết với phân trang (GET: /api/posts/{postId}/comments)
            postsGroup.MapGet("/{postId:guid}/comments", async (Guid postId, int startIndex, int pageSize, PostService postService) =>
                Results.Ok(await postService.GetCommentsAsync(postId, startIndex, pageSize)))
                .Produces<CommentDto[]>(StatusCodes.Status200OK)
                .WithName("GetPostComments");

            // Endpoint để bật/tắt trạng thái "Like" của bài viết (POST: /api/posts/{postId}/toggle-like)
            postsGroup.MapPost("/{postId:guid}/toggle-like", async (Guid postId, PostService postService, ClaimsPrincipal principal) =>
            {
                var result = await postService.ToggleLikeAsync(postId, principal.GetUser());
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
                .Produces<ApiResult>(StatusCodes.Status200OK)
                .Produces<ApiResult>(StatusCodes.Status400BadRequest)
                .WithName("ToggleLike");

            // Endpoint để bật/tắt trạng thái "Bookmark" của bài viết (POST: /api/posts/{postId}/toggle-bookmark)
            postsGroup.MapPost("/{postId:guid}/toggle-bookmark", async (Guid postId, PostService postService, ClaimsPrincipal principal) =>
            {
                var result = await postService.ToggleBookmarkAsync(postId, principal.GetUser());
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
                .Produces<ApiResult>(StatusCodes.Status200OK)
                .Produces<ApiResult>(StatusCodes.Status400BadRequest)
                .WithName("ToggleBookmark");

            // Endpoint để xóa bài viết (DELETE: /api/posts/{postId})
            postsGroup.MapDelete("/{postId:guid}/DeletePost", async (Guid postId, PostService postService, ClaimsPrincipal principal) =>
            {
                var result = await postService.DeletePostAsync(postId, principal.GetUser());
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
                .Produces<ApiResult>(StatusCodes.Status200OK)
                .Produces<ApiResult>(StatusCodes.Status400BadRequest)
                .WithName("DeletePost");

            return app;
        }

        
       
    }
}