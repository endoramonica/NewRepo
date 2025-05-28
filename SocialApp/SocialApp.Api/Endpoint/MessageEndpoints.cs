using SocialApp.Api.Services;
using SocialApp.Api.ServiceInterface;
using SocialAppLibrary.Shared.Dtos;

namespace SocialApp.Api.Endpoints
{
    public static class MessageEndpoints
    {
        public static IEndpointRouteBuilder MapMessageEndpoints(this IEndpointRouteBuilder app)
        {
            var messageGroup = app.MapGroup("/api/message")
                                 .WithTags("Message");

            // Thêm tin nhắn mới
            messageGroup.MapPost("/", async (SendMessageDto dto, IMessageService messageService) =>
            {
                var result = await messageService.AddMessageAsync(dto.FromUserId, dto.ToUserId, dto.Content);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .Produces<ApiResult<int>>()
            .WithName("Message-Send")
            .RequireAuthorization();
            /* Tạo endpoint POST /api/message để gửi tin nhắn mới.
             * Nhận SendMessageDto từ body, gọi IMessageService.AddMessageAsync,
             * trả về số bản ghi được lưu trong ApiResult<int> nếu thành công,
             * hoặc lỗi (400) nếu thất bại. Yêu cầu xác thực. */

            // Lấy tin nhắn mới nhất với bạn bè
            messageGroup.MapGet("/latest/{userId:guid}", async (Guid userId, IMessageService messageService) =>
            {
                var result = await messageService.GetLastestMessageAsync(userId);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .Produces<ApiResult<IEnumerable<LastestMessageDto>>>()
            .WithName("Message-GetLatest")
            .RequireAuthorization();
            /* Tạo endpoint GET /api/message/latest/{userId} để lấy tin nhắn mới nhất với từng bạn bè.
             * Nhận userId từ URL, gọi IMessageService.GetLastestMessageAsync,
             * trả về danh sách LastestMessageDto trong ApiResult<IEnumerable<LastestMessageDto>> nếu thành công,
             * hoặc lỗi (400) nếu thất bại. Yêu cầu xác thực. */

            // Lấy lịch sử tin nhắn giữa hai người dùng
            messageGroup.MapGet("/{fromUserId:guid}/{toUserId:guid}", async (Guid fromUserId, Guid toUserId, IMessageService messageService) =>
            {
                var result = await messageService.GetMessagesAsync(fromUserId, toUserId);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .Produces<ApiResult<IEnumerable<MessageDto>>>()
            .WithName("Message-GetHistory")
            .RequireAuthorization();
            /* Tạo endpoint GET /api/message/{fromUserId}/{toUserId} để lấy lịch sử tin nhắn.
             * Nhận fromUserId và toUserId từ URL, gọi IMessageService.GetMessagesAsync,
             * trả về danh sách MessageDto trong ApiResult<IEnumerable<MessageDto>> nếu thành công,
             * hoặc lỗi (400) nếu thất bại. Yêu cầu xác thực. */

            // Khởi tạo cuộc trò chuyện
            messageGroup.MapPost("/initialize", async (MessageInitializeRequestDto request, IMessageService messageService, AuthService authService) =>
            {
                try
                {
                    var friendInfoResult = await authService.GetUserByIdAsync(request.ToUserId);
                    if (!friendInfoResult.IsSuccess)
                        return Results.BadRequest(friendInfoResult.Error);

                    var messagesResult = await messageService.GetMessagesAsync(request.FromUserId, request.ToUserId);
                    if (!messagesResult.IsSuccess)
                        return Results.BadRequest(messagesResult.Error);

                    var response = new MessageInitializeResponseDto
                    {
                        FriendInfo = friendInfoResult.Data!,
                        Messages = messagesResult.Data!
                    };

                    return Results.Ok(ApiResult<MessageInitializeResponseDto>.Success(response));
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ApiResult<MessageInitializeResponseDto>.Fail(ex.Message));
                }
            })
            .Produces<ApiResult<MessageInitializeResponseDto>>()
            .WithName("Message-Initialize")
            .RequireAuthorization();
            /* Tạo endpoint POST /api/message/initialize để khởi tạo cuộc trò chuyện.
             * Nhận MessageInitializeRequestDto từ body, gọi IAuthService.GetUserByIdAsync và IMessageService.GetMessagesAsync,
             * trả về MessageInitializeResponseDto trong ApiResult<MessageInitializeResponseDto> nếu thành công,
             * hoặc lỗi (400) nếu thất bại. Yêu cầu xác thực. */

            return app;
        }
    }
}