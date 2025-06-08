using Microsoft.AspNetCore.Mvc;
using SocialApp.Api.Services;
using SocialAppLibrary.Shared.Dtos;
using System.Security.Claims;

namespace SocialApp.Api.Endpoint
{
    public static class FollowEndpoints
    {
        public static IEndpointRouteBuilder MapFollowEndpoints(this IEndpointRouteBuilder app)
        {
            var followGroup = app.MapGroup("/api/follow")
                .RequireAuthorization()
                .WithTags("Follow");

            followGroup.MapPost("", async ([FromServices] IFollowService followService, ClaimsPrincipal principal, [FromQuery] Guid followingId) =>
            {
                var result = await followService.FollowAsync(principal.GetUserId(), followingId);
                return Results.Ok(result);
            })
            .Produces<ApiResult<string>>()
            .WithName("Follow");

            followGroup.MapDelete("", async ([FromServices] IFollowService followService, ClaimsPrincipal principal, [FromQuery] Guid followingId) =>
            {
                var result = await followService.UnfollowAsync(principal.GetUserId(), followingId);
                return Results.Ok(result);
            })
            .Produces<ApiResult<string>>()
            .WithName("Unfollow");

            followGroup.MapGet("/followers", async ([FromServices] IFollowService followService, ClaimsPrincipal principal, int startIndex, int pageSize) =>
            {
                var result = await followService.GetFollowersAsync(principal.GetUserId(), startIndex, pageSize);
                return Results.Ok(result);
            })
            .Produces<ApiResult<LoggedInUser[]>>()
            .WithName("GetFollowers");

            followGroup.MapGet("/following", async ([FromServices] IFollowService followService, ClaimsPrincipal principal, int startIndex, int pageSize) =>
            {
                var result = await followService.GetFollowingAsync(principal.GetUserId(), startIndex, pageSize);
                return Results.Ok(result);
            })
            .Produces<ApiResult<LoggedInUser[]>>()
            .WithName("GetFollowing");

            followGroup.MapGet("/followers/search", async ([FromServices] IFollowService followService, ClaimsPrincipal principal, [FromQuery] string q, int startIndex, int pageSize) =>
            {
                var result = await followService.SearchFollowersAsync(principal.GetUserId(), q, startIndex, pageSize);
                return Results.Ok(result);
            })
            .Produces<ApiResult<LoggedInUser[]>>()
            .WithName("SearchFollowers");
            // Thêm endpoint mới cho IsFollowing
            followGroup.MapGet("/is-following", async ([FromServices] IFollowService followService, ClaimsPrincipal principal, [FromQuery] Guid followingId) =>
            {
                var followerId = principal.GetUserId();
                var result = await followService.IsFollowingAsync(followerId, followingId);
                return Results.Ok(result);
            })
            .Produces<ApiResult<bool>>()
            .WithName("IsFollowing");
            followGroup.MapGet("/follower-count", async ([FromServices] IFollowService followService, ClaimsPrincipal principal) =>
            {
                var userId = principal.GetUserId();
                var result = await followService.GetFollowerCountAsync(userId);
                return Results.Ok(result);
            })
            .Produces<ApiResult<int>>()
            .WithName("GetFollowerCount");

            followGroup.MapGet("/following-count", async ([FromServices] IFollowService followService, ClaimsPrincipal principal) =>
            {
                var userId = principal.GetUserId();
                var result = await followService.GetFollowingCountAsync(userId);
                return Results.Ok(result);
            })
            .Produces<ApiResult<int>>()
            .WithName("GetFollowingCount");
            return app;
        }
    }
}