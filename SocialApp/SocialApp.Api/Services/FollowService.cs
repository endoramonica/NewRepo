using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SocialApp.Api.Data;
using SocialApp.Api.Data.Entities;
using SocialApp.Api.Hubs;
using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.IHub;

namespace SocialApp.Api.Services
{
    public interface IFollowService
    {
        Task<ApiResult<string>> FollowAsync(Guid followerId, Guid followingId);
        Task<ApiResult<string>> UnfollowAsync(Guid followerId, Guid followingId);
        Task<ApiResult<LoggedInUser[]>> GetFollowersAsync(Guid userId, int startIndex, int pageSize);
        Task<ApiResult<LoggedInUser[]>> GetFollowingAsync(Guid userId, int startIndex, int pageSize);
        Task<ApiResult<LoggedInUser[]>> SearchFollowersAsync(Guid userId, string query, int startIndex, int pageSize);
        Task<ApiResult<bool>> IsFollowingAsync(Guid followerId, Guid followingId);
        Task<ApiResult<int>> GetFollowerCountAsync(Guid userId);
        Task<ApiResult<int>> GetFollowingCountAsync(Guid userId);
    }

    public class FollowService : IFollowService
    {
        private readonly DataContext _context;
        private readonly IHubContext<SocialHubs, ISocialHubClient> _hubContext;

        public FollowService(DataContext context, IHubContext<SocialHubs, ISocialHubClient> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        public async Task<ApiResult<bool>> IsFollowingAsync(Guid followerId, Guid followingId)
        {
            try
            {
                var isFollowing = await _context.Follows
                    .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId && f.IsActive);
                return ApiResult<bool>.Success(isFollowing);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }
        public async Task<ApiResult<string>> FollowAsync(Guid followerId, Guid followingId)
        {
            if (followerId == followingId)
                return ApiResult<string>.Fail("Cannot follow yourself");

            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            try
            {
                if (existingFollow != null)
                {
                    if (!existingFollow.IsActive)
                    {
                        existingFollow.IsActive = true;
                        existingFollow.FollowedOn = DateTime.UtcNow;
                        _context.Follows.Update(existingFollow);
                    }
                }
                else
                {
                    var follow = new Follow
                    {
                        Id = Guid.NewGuid(),
                        FollowerId = followerId,
                        FollowingId = followingId,
                        FollowedOn = DateTime.UtcNow,
                        IsActive = true
                    };
                    _context.Follows.Add(follow);
                }

                await _context.SaveChangesAsync();
                var follower = await _context.Users.FindAsync(followerId);
                var following = await _context.Users.FindAsync(followingId);
                // Gửi thông báo qua SignalR
                await _hubContext.Clients.User(followingId.ToString())
                    .FollowNotification(new FollowNotificationDto(
                        followerId, followingId,
                        $"{follower.Name} followed you",
                        new LoggedInUser(follower.Id, follower.Name, follower.Email, follower.PhotoUrl)
                    ));
                return ApiResult<string>.Success("Followed successfully");
            }
            catch (Exception ex)
            {
                return ApiResult<string>.Fail(ex.Message);
            }
        }

        public async Task<ApiResult<string>> UnfollowAsync(Guid followerId, Guid followingId)
        {
            var follow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            if (follow == null || !follow.IsActive)
                return ApiResult<string>.Fail("Not following this user");

            try
            {
                follow.IsActive = false;
                _context.Follows.Update(follow);
                await _context.SaveChangesAsync();
                return ApiResult<string>.Success("Unfollowed successfully");
            }
            catch (Exception ex)
            {
                return ApiResult<string>.Fail(ex.Message);
            }
        }

        public async Task<ApiResult<LoggedInUser[]>> GetFollowersAsync(Guid userId, int startIndex, int pageSize)
        {
            try
            {
                var followers = await _context.Follows
                    .Where(f => f.FollowingId == userId && f.IsActive)
                    .OrderBy(f => f.FollowedOn)
                    .Skip(startIndex)
                    .Take(pageSize)
                    .Include(f => f.Follower)
                    .Select(f => new LoggedInUser(f.Follower.Id, f.Follower.Name, f.Follower.Email, f.Follower.PhotoUrl))
                    .ToArrayAsync();
                return ApiResult<LoggedInUser[]>.Success(followers);
            }
            catch (Exception ex)
            {
                return ApiResult<LoggedInUser[]>.Fail(ex.Message);
            }
        }

        public async Task<ApiResult<LoggedInUser[]>> GetFollowingAsync(Guid userId, int startIndex, int pageSize)
        {
            try
            {
                var following = await _context.Follows
                    .Where(f => f.FollowerId == userId && f.IsActive)
                    .OrderBy(f => f.FollowedOn)
                    .Skip(startIndex)
                    .Take(pageSize)
                    .Include(f => f.Following)
                    .Select(f => new LoggedInUser(f.Following.Id, f.Following.Name, f.Following.Email, f.Following.PhotoUrl))
                    .ToArrayAsync();
                return ApiResult<LoggedInUser[]>.Success(following);
            }
            catch (Exception ex)
            {
                return ApiResult<LoggedInUser[]>.Fail(ex.Message);
            }
        }

        public async Task<ApiResult<LoggedInUser[]>> SearchFollowersAsync(Guid userId, string query, int startIndex, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(query))
                return ApiResult<LoggedInUser[]>.Success(Array.Empty<LoggedInUser>());

            try
            {
                var followers = await _context.Follows
                    .Where(f => f.FollowingId == userId && f.IsActive)
                    .Include(f => f.Follower)
                    .Where(f => EF.Functions.Contains(f.Follower.Name, $"\"{query}*\"") || EF.Functions.Contains(f.Follower.Email, $"\"{query}*\""))
                    .OrderBy(f => f.FollowedOn)
                    .Skip(startIndex)
                    .Take(pageSize)
                    .Select(f => new LoggedInUser(f.Follower.Id, f.Follower.Name, f.Follower.Email, f.Follower.PhotoUrl))
                    .ToArrayAsync();
                return ApiResult<LoggedInUser[]>.Success(followers);
            }
            catch (Exception ex)
            {
                return ApiResult<LoggedInUser[]>.Fail(ex.Message);
            }
        }
        public async Task<ApiResult<int>> GetFollowerCountAsync(Guid userId)
        {
            try
            {
                var count = await _context.Follows
                    .Where(f => f.FollowingId == userId && f.IsActive)
                    .CountAsync();
                return ApiResult<int>.Success(count);
            }
            catch (Exception ex)
            {
                return ApiResult<int>.Fail(ex.Message);
            }
        }

        public async Task<ApiResult<int>> GetFollowingCountAsync(Guid userId)
        {
            try
            {
                var count = await _context.Follows
                    .Where(f => f.FollowerId == userId && f.IsActive)
                    .CountAsync();
                return ApiResult<int>.Success(count);
            }
            catch (Exception ex)
            {
                return ApiResult<int>.Fail(ex.Message);
            }
        }

    }
}