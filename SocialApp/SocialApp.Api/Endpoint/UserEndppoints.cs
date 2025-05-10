using Microsoft.AspNetCore.Mvc;
using SocialApp.Api.Services;
using SocialAppLibrary.Shared.Dtos;
using System.Security.Claims;
//#22
namespace SocialApp.Api.Endpoint
{
    public static class UserEndppoints
    {
        public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            var userGroup = app.MapGroup("/api/user")
                .RequireAuthorization()
                .WithTags("User");
            //POST / change - photo: Thay đổi ảnh đại diện người dùng
            userGroup.MapPost("/change-photo", async (HttpContext context, UserService userService, ClaimsPrincipal principal) =>
            {
                var file = context.Request.Form.Files.FirstOrDefault();
                if (file == null) return Results.BadRequest("No photo uploaded.");

                var result = await userService.ChangePhotoAsync(file, principal.GetUserId());
                return Results.Ok(result);
            })
                .DisableAntiforgery()
                .Produces<ApiResult>()
                .WithName("ChangePhoto");

            // GET /posts: Lấy danh sách bài viết của người dùng
            userGroup.MapGet("/posts", async (HttpContext context, [FromServices] UserService userService, int startIndex, int pageSize) =>
            {
                var userId = context.User.GetUserId();
                var result = await userService.GetUserPostsAsync(startIndex, pageSize, userId);
                return Results.Ok(result);
            })
            .Produces<PostDto[]>()
            .WithName("GetUserPosts");

            // GET /bookmarked-posts: Lấy danh sách bài viết đã đánh dấu của người dùng
            userGroup.MapGet("/bookmarked-posts", async (int startIndex, int pageSize, [FromServices] UserService userService, ClaimsPrincipal principal) =>
                Results.Ok(await userService.GetUserBookmarkedPostsAsync(startIndex, pageSize, principal.GetUserId())))
                .Produces<PostDto[]>()
                .WithName("GetUserBookmarkedPosts");
            /* Các endpoints đều sử dụng dependency injection để cung cấp UserService và thông tin người dùng (ClaimsPrincipal).
                Các endpoint sử dụng phân trang (startIndex, pageSize) để xử lý lượng lớn dữ liệu.
                Endpoint /bookmarked-posts có tên trùng với endpoint /posts.
                Các endpoints đều trả về responses được bao bọc trong Results.Ok, cho thấy đây là responses thành công (200 OK).
                Các endpoints đều chỉ định kiểu response trả về sử dụng .Produces<>(). */

            return app;
        }

    }
}
