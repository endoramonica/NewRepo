using Microsoft.AspNetCore.SignalR;
using SocialApp.Api.Services;
using SocialAppLibrary.Shared.Dtos;
using SocialApp.Api.ServiceInterface.ChatService;

namespace SocialApp.Api.Endpoint
{
    public static class ChatEndpoints
    {
        public static IEndpointRouteBuilder MapChatEndpoints(this IEndpointRouteBuilder app)
        {
            var chatGroup = app.MapGroup("/api/chat")
                               .WithTags("Chat")
                               .RequireAuthorization();

            // Gửi tin nhắn đến một người dùng
            chatGroup.MapPost("/send", async (
                SendMessageDto request,
                IChatService chatService) =>
            {
                var result = await chatService.SendMessageToUserAsync(request.FromUserId, request.ToUserId, request.Content);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .Produces<ApiResult<bool>>()
            .WithName("Chat-SendToUser");

            // Gửi tin nhắn đến tất cả người dùng
            chatGroup.MapPost("/broadcast", async (
                BroadcastMessageRequest request,
                IChatService chatService) =>
            {
                var result = await chatService.SendMessageToAllAsync(request.Content);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .Produces<ApiResult<bool>>()
            .WithName("Chat-Broadcast");

            // Thêm kết nối
            chatGroup.MapPost("/connect", async (
                ConnectionRequest request,
                IChatService chatService) =>
            {
                await chatService.AddConnectionAsync(request.UserId, request.ConnectionId);
                return Results.Ok(ApiResult<bool>.Success(true));
            })
            .Produces<ApiResult<bool>>()
            .WithName("Chat-AddConnection");

            // Xóa kết nối
            chatGroup.MapPost("/disconnect", async (
                DisconnectionRequest request,
                IChatService chatService) =>
            {
                await chatService.RemoveConnectionAsync(request.UserId);
                return Results.Ok(ApiResult<bool>.Success(true));
            })
            .Produces<ApiResult<bool>>()
            .WithName("Chat-RemoveConnection");

            return app;
        }
    }
}