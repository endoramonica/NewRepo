using SocialApp.Api.ServiceInterface;
using SocialApp.Api.Services;
using SocialAppLibrary.Shared.Dtos;

namespace SocialApp.Api.Endpoints
{
    public static class UserFriendEndpoints
    {
        public static IEndpointRouteBuilder MapUserFriendEndpoints(this IEndpointRouteBuilder app)
        {
            var userGroup = app.MapGroup("/api/user")
                               .WithTags("User");

            // Lấy danh sách bạn bè của người dùng
            userGroup.MapGet("/friends/{userId:guid}", async (Guid userId, IUserFriendService userFriendService) =>
            {
                var result = await userFriendService.GetUserFriendsAsync(userId);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .Produces<ApiResult<IEnumerable<UserDto>>>()
            .WithName("User-GetFriends")
            .RequireAuthorization();
            /* Tạo endpoint GET /api/user/friends/{userId} để lấy danh sách bạn bè.
             * Nhận userId từ URL, gọi IUserFriendService.GetUserFriendsAsync,
             * trả về danh sách UserDto trong ApiResult<IEnumerable<UserDto>> nếu thành công,
             * hoặc lỗi (400) nếu thất bại. Yêu cầu xác thực. */

            return app;
        }
    }
}