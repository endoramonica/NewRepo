


using Microsoft.EntityFrameworkCore;
using SocialApp.Api.Data;
using SocialAppLibrary.Shared.Dtos;

namespace SocialApp.Api.Services
{
    public class UserService
    {
        private readonly DataContext _dataContext;
        private readonly PhotoUploadService _photoUploadService;

        public UserService( DataContext dataContext , PhotoUploadService photoUploadService) 
        { 
            _dataContext = dataContext;
           _photoUploadService = photoUploadService;
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

        public async Task<PostDto[]> GetUserPostsAsync(int startIndex , int pageSize , Guid currentUserId)
        {
            // Executes the stored procedure 'GetPosts' to retrieve paginated posts for the user.
            var posts = await _dataContext.Set<PostDto>()
             .FromSqlInterpolated($"EXEC GetUserPosts @StartIndex={startIndex}, @PageSize={pageSize}, @CurrentUserId = {currentUserId}")
             .ToArrayAsync();

                    return posts;
        }
        public async Task<PostDto[]> GetUserBookmarkedPostsAsync(int startIndex , int pageSize , Guid currentUserId)
        {
            // Executes the stored procedure 'GetBookmarkedPosts' to retrieve paginated bookmarked posts for the user.
            var posts = await _dataContext.Set<PostDto>()
     .FromSqlInterpolated($"EXEC GetUserBookmarkedPosts @StartIndex={startIndex}, @PageSize={pageSize}, @CurrentUserId = {currentUserId}")
     .ToArrayAsync();

            return posts;
        }

    }
}
