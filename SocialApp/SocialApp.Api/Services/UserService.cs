


using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SocialApp.Api.Data;
using SocialApp.Api.Hubs;
using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.IHub;

namespace SocialApp.Api.Services
{
    public class UserService
    {
        private readonly DataContext _dataContext;
        private readonly PhotoUploadService _photoUploadService;
        private readonly IHubContext<SocialHubs, ISocialHubClient> _hubContext;

        public UserService(DataContext dataContext, PhotoUploadService photoUploadService, IHubContext<SocialHubs, ISocialHubClient> hubContext)
        {
            _dataContext = dataContext;
            _photoUploadService = photoUploadService;
            _hubContext = hubContext;
        }
        public async Task<ApiResult<string>> ChangePhotoAsync(IFormFile photo, Guid currentUserId)
        {
            // 🔹 1. Tìm user theo ID trong database
            var user = await _dataContext.Users.FindAsync(currentUserId);

            // 🔹 2. Kiểm tra nếu user không tồn tại
            if (user is null)
            {
                return ApiResult<string>.Fail("User not found"); // Trả về lỗi nếu không tìm thấy user
            }

            try
            {
                // 🔹 3. Lưu đường dẫn ảnh hiện tại (để xóa nếu cần)
                var existingPhotoPath = user.PhotoPath;

                // 🔹 4. Upload ảnh mới lên server/cloud và lấy đường dẫn
                // SavePhotoAsync() sẽ lưu ảnh vào thư mục "uploads/images/users" 
                // và trả về (đường dẫn trong hệ thống, URL truy cập ảnh)
                (user.PhotoPath, user.PhotoUrl) = await _photoUploadService.SavePhotoAsync(photo, "uploads", "images", "users");

                // 🔹 5. Cập nhật thông tin user trong database
                _dataContext.Users.Update(user);
                await _dataContext.SaveChangesAsync();
                await _hubContext.Clients.All.UserPhotoChange(new UserPhotoChange(currentUserId, user.PhotoUrl)); // Gửi thông báo đến tất cả client về việc thay đổi ảnh đại diện của user
                // 🔹 6. Nếu user đã có ảnh trước đó, xóa ảnh cũ khỏi hệ thống
                if (!string.IsNullOrWhiteSpace(existingPhotoPath) && File.Exists(existingPhotoPath))
                {
                    File.Delete(existingPhotoPath);
                }

                // 🔹 7. Trả về URL của ảnh mới
                return ApiResult<string>.Success(user.PhotoUrl);
            }
            catch (Exception ex)
            {
                // 🔹 8. Nếu có lỗi xảy ra, trả về thông báo lỗi
                return ApiResult<string>.Fail(ex.Message);
            }
        }

        public async Task<PostDto[]> GetUserPostsAsync(int startIndex, int pageSize, Guid currentUserId)
        {
            // Executes the stored procedure 'GetPosts' to retrieve paginated posts for the user.
            var posts = await _dataContext.Set<PostDto>()
             .FromSqlInterpolated($"EXEC GetUserPosts @StartIndex={startIndex}, @PageSize={pageSize}, @CurrentUserId = {currentUserId}")
             .ToArrayAsync();

            return posts;
        }
        public async Task<PostDto[]> GetUserBookmarkedPostsAsync(int startIndex, int pageSize, Guid currentUserId)
        {
            // Executes the stored procedure 'GetBookmarkedPosts' to retrieve paginated bookmarked posts for the user.
            var posts = await _dataContext.Set<PostDto>()
     .FromSqlInterpolated($"EXEC GetUserBookmarkedPosts @StartIndex={startIndex}, @PageSize={pageSize}, @CurrentUserId = {currentUserId}")
     .ToArrayAsync();

            return posts;
        }
        public async Task<PostDto[]> GetUserLikedPostsAsync(int startIndex, int pageSize, Guid currentUserId)
        {
            // Executes the stored procedure 'GetUserLikedPosts' to retrieve paginated liked posts for the user.
            var posts = await _dataContext.Set<PostDto>()
                .FromSqlInterpolated($"EXEC GetUserLikedPosts @StartIndex={startIndex}, @PageSize={pageSize}, @CurrentUserId={currentUserId}")
                .ToArrayAsync();

            return posts;
        }
        public async Task<NotificationDto[]> GetNotificationsAsync(int startIndex, int pageSize, Guid currentUserId)
        {
            var notifications = await _dataContext.Notifications
                .Where(n => n.ForUserId == currentUserId)
                .OrderByDescending(n => n.When)
                .Skip(startIndex)
                .Take(pageSize)
                .Select(n => new NotificationDto(
                    n.ForUserId,
                    n.Text,
                    n.When,
                    n.PostId,
                    n.User.PhotoPath // Giả sử Notification có navigation property User
                ))
                .ToArrayAsync();

            return notifications;
        }
    }

    }
