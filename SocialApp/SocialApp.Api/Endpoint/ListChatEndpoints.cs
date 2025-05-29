using SocialApp.Api.ServiceInterface;
using SocialApp.Api.Services;
using SocialAppLibrary.Shared.Dtos;
using System.Security.Claims;

namespace SocialApp.Api.Endpoints
{
    public static class ListChatEndpoints
    {
        public static IEndpointRouteBuilder MapListChatEndpoints(this IEndpointRouteBuilder app)
        {
            var chatGroup = app.MapGroup("/api/listchat")
                               .WithTags("ListChat");

            // Khởi tạo dữ liệu chat
            chatGroup.MapPost("/initialize", async (HttpContext context, AuthService authService, IUserFriendService userFriendService, IMessageService messageService) =>
            {
                try
                {
                    // Lấy userId từ JWT token
                    var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                        return Results.Unauthorized();

                    var userResult = await authService.GetUserByIdAsync(userId);
                    if (!userResult.IsSuccess || userResult.Data == null)
                        return Results.BadRequest(userResult.Error);

                    var friendsResult = await userFriendService.GetUserFriendsAsync(userId);
                    if (!friendsResult.IsSuccess || friendsResult.Data == null)
                        return Results.BadRequest(friendsResult.Error);

                    var messagesResult = await messageService.GetLastestMessageAsync(userId);
                    if (!messagesResult.IsSuccess || messagesResult.Data == null)
                        return Results.BadRequest(messagesResult.Error);

                    var response = new ListChatInitializeResponseDto
                    {
                        User = userResult.Data,
                        UserFriends = friendsResult.Data,
                        LastestMessages = messagesResult.Data
                    };

                    return Results.Ok(ApiResult<ListChatInitializeResponseDto>.Success(response));
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ApiResult<ListChatInitializeResponseDto>.Fail(ex.Message));
                }
            })
            .Produces<ApiResult<ListChatInitializeResponseDto>>()
            .WithName("ListChat-Initialize")
            .RequireAuthorization();
            /* Tạo endpoint POST /api/chat/initialize để khởi tạo dữ liệu chat.
             * Lấy userId từ JWT token, gọi IAuthService.GetUserByIdAsync, IUserFriendService.GetUserFriendsAsync,
             * và IMessageService.GetLastestMessageAsync, trả về ChatInitializeResponseDto chứa thông tin người dùng,
             * danh sách bạn bè, và tin nhắn mới nhất trong ApiResult<ChatInitializeResponseDto> nếu thành công,
             * hoặc lỗi (400) nếu thất bại. Yêu cầu xác thực. */

            return app;
        }
    }
}