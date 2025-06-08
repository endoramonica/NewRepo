using Microsoft.AspNetCore.Mvc; // Thêm namespace này để dùng [FromBody] và [FromServices]
using SocialApp.Api.ServiceInterface;
using SocialApp.Api.Services;
using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.Dtos.ChatDto;

namespace SocialApp.Api.Endpoints
{
    public static class UserFriendEndpoints
    {
        public static IEndpointRouteBuilder MapUserFriendEndpoints(this IEndpointRouteBuilder app)
        {
            var userGroup = app.MapGroup("/api/user")
                               .WithTags("User");

            // Lấy danh sách bạn bè của người dùng
            userGroup.MapGet("/friends/{userId:guid}", async ([FromServices] IUserFriendService userFriendService, Guid userId) =>
            {
                var result = await userFriendService.GetUserFriendsAsync(userId);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .Produces<ApiResult<IEnumerable<UserDto>>>()
            .WithName("User-GetFriends")
            .RequireAuthorization();

            // Gửi lời mời kết bạn
            userGroup.MapPost("/friends/request", async ([FromBody] FriendRequestDto dto, [FromServices] IUserFriendService userFriendService) =>
            {
                var result = await userFriendService.SendFriendRequestAsync(dto.FromUserId, dto.ToUserId);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .Produces<ApiResult<bool>>()
            .WithName("User-SendFriendRequest")
            .RequireAuthorization();

            // Chấp nhận lời mời kết bạn
            userGroup.MapPut("/friends/accept", async ([FromBody] FriendActionDto dto, [FromServices] IUserFriendService userFriendService) =>
            {
                var result = await userFriendService.AcceptFriendRequestAsync(dto.UserId, dto.FriendId);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .Produces<ApiResult<bool>>()
            .WithName("User-AcceptFriendRequest")
            .RequireAuthorization();

            // Từ chối lời mời kết bạn
            userGroup.MapPut("/friends/reject", async ([FromBody] FriendActionDto dto, [FromServices] IUserFriendService userFriendService) =>
            {
                var result = await userFriendService.RejectFriendRequestAsync(dto.UserId, dto.FriendId);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .Produces<ApiResult<bool>>()
            .WithName("User-RejectFriendRequest")
            .RequireAuthorization();

            // Lấy danh sách lời mời đang chờ
            userGroup.MapGet("/friends/pending/{userId:guid}", async ([FromServices] IUserFriendService userFriendService, Guid userId) =>
            {
                var result = await userFriendService.GetPendingFriendRequestsAsync(userId);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .Produces<ApiResult<IEnumerable<UserDto>>>()
            .WithName("User-GetPendingFriendRequests")
            .RequireAuthorization();

            // Thêm endpoint để xóa bạn bè (nếu cần)
            userGroup.MapDelete("/friends/remove", async ([FromBody] FriendActionDto dto, [FromServices] IUserFriendService userFriendService) =>
            {
                var result = await userFriendService.RemoveFriendAsync(dto.UserId, dto.FriendId);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .Produces<ApiResult<bool>>()
            .WithName("User-RemoveFriend")
            .RequireAuthorization();

            return app;
        }
    }
}