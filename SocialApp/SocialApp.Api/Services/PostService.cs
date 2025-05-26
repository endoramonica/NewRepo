using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SocialApp.Api.Data;
using SocialApp.Api.Data.Entities;
using SocialApp.Api.Hubs;
using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.IHub;
using System.Security.Cryptography;

namespace SocialApp.Api.Services
{
    public class PostService
    {
        private readonly DataContext _dataContext; // Quản lý kết nối và tương tác với database
        private readonly PhotoUploadService _photoUploadService;
        private readonly ILogger<PostService> _logger;
        private readonly IHubContext<SocialHubs, ISocialHubClient> _hubContext;
        private readonly IHttpContextAccessor _httpContextAccessor; // Truy cập HttpContext để lấy thông tin người dùng từ token

        // Constructor: Inject các dịch vụ cần thiết để sử dụng trong class
        public PostService(DataContext dataContext,
            PhotoUploadService photoUploadService,
            ILogger<PostService> logger,
            IHubContext<SocialHubs, ISocialHubClient> hubContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _photoUploadService = photoUploadService;
            _logger = logger;
            _hubContext = hubContext;

            _httpContextAccessor = httpContextAccessor;
        }

        // Phương thức lấy UserId từ token của người dùng
        private Guid GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
            return userIdClaim != null ? Guid.Parse(userIdClaim) : throw new Exception("UserId not found");
        }

        // Lưu bài viết (cả tạo mới và chỉnh sửa bài viết)
        //public async Task<ApiResult> SavePostAsync(SavePost dto, Guid userId)
        //{
        //    if (dto == null)
        //    {
        //        return ApiResult.Fail("Dữ liệu không hợp lệ");
        //    }

        //    string? _existingPhotoPath = null;

        //    try
        //    {
        //        if (dto.PostId == null)
        //        {
        //            // Tạo bài viết mới
        //            var post = new Post
        //            {
        //                Content = dto.Content,
        //                PostedOn = DateTime.UtcNow,
        //                UserId = userId,
        //            };

        //            // Lưu ảnh nếu có
        //            if (dto.Photo is not null)
        //            {
        //                var (photoPath, photoUrl) = await _photoUploadService.SavePhotoAsync(dto.Photo, "uploads", "images", "users", userId.ToString(), "posts");
        //                post.PhotoPath = photoPath;
        //                post.PhotoUrl = photoUrl;
        //                //post.PhotoHash = ComputeSha256Hash(dto.Photo); // Tính hash từ file ảnh
        //            }

        //            _dataContext.Posts.Add(post);
        //        }
        //        else
        //        {
        //            // Tìm bài viết cần chỉnh sửa
        //            var post = await _dataContext.Posts.FindAsync(dto.PostId);
        //            if (post is null)
        //            {
        //                _logger.LogWarning("Không tìm thấy bài viết với ID: {PostId}", dto.PostId);
        //                return ApiResult.Fail("Bài viết không tồn tại");
        //            }

        //            // Kiểm tra quyền chỉnh sửa bài viết
        //            if (post.UserId != userId)
        //            {
        //                return ApiResult.Fail("Bạn không có quyền chỉnh sửa bài viết này");
        //            }

        //            // Cập nhật nội dung bài viết
        //            post.Content = dto.Content;
        //            post.ModifiedOn = DateTime.UtcNow;

        //            // Xử lý ảnh
        //            if (dto.Photo is not null)
        //            {
        //               // var hash = ComputeSha256Hash(dto.Photo);

        //                // Kiểm tra xem có bài viết nào đã sử dụng ảnh này không
        //                //var existingPost = await _dataContext.Posts.FirstOrDefaultAsync(p => p.PhotoHash == hash);

        //                //if (existingPost != null)
        //                //{
        //                //    // Nếu ảnh đã tồn tại, dùng lại thông tin ảnh cũ
        //                //    post.PhotoPath = existingPost.PhotoPath;
        //                //    post.PhotoUrl = existingPost.PhotoUrl;
        //                //    //post.PhotoHash = existingPost.PhotoHash;
        //                //}
        //                //else
        //                {
        //                    _existingPhotoPath = post.PhotoPath;

        //                    // Lưu ảnh mới
        //                    var (photoPath, photoUrl) = await _photoUploadService.SavePhotoAsync(dto.Photo, "uploads", "images", "users", userId.ToString(), "posts");
        //                    post.PhotoPath = photoPath;
        //                    post.PhotoUrl = photoUrl;
        //                   // post.PhotoHash = hash; // Gán hash đã tính
        //                }
        //            }
        //            else if (dto.IsExistingPhotoRemoved)
        //            {
        //                _existingPhotoPath = post.PhotoPath;
        //                post.PhotoPath = null;
        //                post.PhotoUrl = null;
        //                //post.PhotoHash = null;
        //            }


        //            _dataContext.Posts.Update(post);
        //        }

        //        await _dataContext.SaveChangesAsync();

        //        // Xóa ảnh cũ (nếu có) sau khi lưu thành công
        //        if (!string.IsNullOrWhiteSpace(_existingPhotoPath))
        //        {
        //            SafeDeleteFile(_existingPhotoPath);
        //        }

        //        return ApiResult.Success();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        return ApiResult.Fail("Bài viết đã được chỉnh sửa bởi người khác. Hãy tải lại và thử lại.");
        //    }
        //    catch (IOException)
        //    {
        //        return ApiResult.Fail("Không thể xóa ảnh cũ do lỗi hệ thống.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return ApiResult.Fail($"Lỗi không xác định: {ex.Message}");
        //    }
        //}


        // Hàm xóa file an toàn, xử lý race condition
        public async Task<ApiResult<PostDto>> SavePostAsync(SavePost dto, LoggedInUser user)
        {
            if (dto == null)
            {
                return ApiResult<PostDto>.Fail("Dữ liệu không hợp lệ");
            }

            string? _existingPhotoPath = null;
            Post? post = null;
            bool sendNotification = false;
            try
            {
                if (dto.PostId == Guid.Empty)
                {
                    // Tạo bài viết mới
                    post = new Post
                    {
                        Content = dto.Content,
                        PostedOn = DateTime.UtcNow,
                        UserId = user.ID,
                    };

                    // Lưu ảnh nếu có
                    if (dto.Photo is not null)
                    {
                        (post.PhotoPath, post.PhotoUrl) = await _photoUploadService.SavePhotoAsync(dto.Photo, "uploads", "images", "users", user.ID.ToString(), "posts");

                    }

                    _dataContext.Posts.Add(post);
                    sendNotification = true;
                }
                else
                {
                    // Tìm bài viết cần chỉnh sửa
                    post = await _dataContext.Posts.FindAsync(dto.PostId);
                    if (post is null)
                    {
                        _logger.LogWarning("Không tìm thấy bài viết với ID: {PostId}", dto.PostId);
                        return ApiResult<PostDto>.Fail("Bài viết không tồn tại");
                    }

                    // Kiểm tra quyền chỉnh sửa bài viết
                    if (post.UserId != user.ID)
                    {
                        return ApiResult<PostDto>.Fail("Bạn không có quyền chỉnh sửa bài viết này");
                    }

                    // Cập nhật nội dung bài viết
                    post.Content = dto.Content;
                    post.ModifiedOn = DateTime.UtcNow;

                    // Xử lý ảnh
                    if (dto.Photo is not null)
                    {
                        _existingPhotoPath = post.PhotoPath;

                        // Lưu ảnh mới
                        (post.PhotoPath, post.PhotoUrl) = await _photoUploadService.SavePhotoAsync(dto.Photo, "uploads", "images", "users", user.ID.ToString(), "posts");

                    }
                    else if (dto.IsExistingPhotoRemoved)
                    {
                        _existingPhotoPath = post.PhotoPath;
                        post.PhotoPath = null;
                        post.PhotoUrl = null;
                    }

                    _dataContext.Posts.Update(post);
                }

                await _dataContext.SaveChangesAsync();

                // Xóa ảnh cũ (nếu có) sau khi lưu thành công
                if (!string.IsNullOrWhiteSpace(_existingPhotoPath))
                {
                    SafeDeleteFile(_existingPhotoPath);
                }
                var postDto = new PostDto
                {
                    Content = post.Content,
                    ModifiedOn = post.ModifiedOn,
                    PhotoUrl = post.PhotoUrl,
                    PostId = post.Id,
                    PostedOn = post.PostedOn,
                    UserId = post.UserId,
                    UserName = user.Name,
                    UserPhotoUrl = user.DisplayPhotoUrl
                };

                if (sendNotification)
                {
                    await _hubContext.Clients.All.PostChange(postDto);
                }
                return ApiResult<PostDto>.Success(postDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                return ApiResult<PostDto>.Fail("Bài viết đã được chỉnh sửa bởi người khác. Hãy tải lại và thử lại.");
            }
            catch (IOException)
            {
                return ApiResult<PostDto>.Fail("Không thể xóa ảnh cũ do lỗi hệ thống.");
            }
            catch (Exception ex)
            {
                return ApiResult<PostDto>.Fail($"Lỗi không xác định: {ex.Message}");
            }
        }

        private void SafeDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
               
            }
            catch (IOException ex)
            {
                // Log lỗi, nhưng không throw exception. Có thể dùng một logger như Serilog hoặc NLog
                Console.Error.WriteLine($"Không thể xóa file {path}: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log lỗi chung
                Console.Error.WriteLine($"Lỗi không xác định khi xóa file {path}: {ex.Message}");
            }

        }

        public class PhotoUploadResult
        {
            public string Path { get; set; }
            public string Url { get; set; }

            public string Hash { get; set; }
        }
        //// hàm băm 
        //public string ComputeSha256Hash(IFormFile file)
        //{
        //    using var sha256 = SHA256.Create();
        //    using var stream = file.OpenReadStream();
        //    var hashBytes = sha256.ComputeHash(stream);
        //    return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        //}



        // Xử lí thủ tục lưu trữ (Stored Procedure)
        /* public async Task<PostDto[]> GetPostsAsync(int startIndex, int pageSize, Guid currentUserId)
         {
             var posts = await _dataContext.Posts
     .Include(p => p.User) // Kết hợp thông tin người dùng (User) của mỗi bài viết
     .Where(p => !p.IsDeleted) // Chỉ lấy các bài viết chưa bị xóa
     .OrderByDescending(p => p.PostedOn) // Sắp xếp theo thời gian đăng bài (mới nhất trước)
     .Select(p => new PostDto // Chuyển đổi kết quả thành đối tượng PostDto
     {
         Content = p.Content,
         ModifiedOn = p.ModifiedOn,
         PhotoUrl = p.PhotoUrl,
         PostId = p.Id,
         PostedOn = p.PostedOn,
         UserId = currentUserId,
         UserName = p.User.Name,
         UserPhotoUrl = p.User.photoUrl
     })
     .Skip(startIndex) // Bỏ qua các bài viết trước startIndex (phân trang)
     .Take(pageSize) // Lấy số lượng bài viết bằng pageSize
     .ToArrayAsync(); // Chuyển kết quả thành mảng
             var postIds = posts.Select(p => p.PostId).ToArray(); // Lấy danh sách ID của các bài viết
             var postsLikedByThisUser = await _dataContext.Likes
                 .Where(l => l.UserId == currentUserId && postIds.Contains(l.PostId)) // Tìm các like của người dùng hiện tại
                 .Select(l => l.PostId) // Chỉ lấy PostId
                 .ToArrayAsync(); // Chuyển kết quả thành mảng
             var postsSavedByThisUser = await _dataContext.Bookmarks
     .Where(l => l.UserId == currentUserId && postIds.Contains(l.PostId)) // Tìm các bookmark của người dùng hiện tại
     .Select(l => l.PostId) // Chỉ lấy PostId
     .ToArrayAsync(); // Chuyển kết quả thành mảng
             foreach (var post in posts)
             {
                 post.IsLiked = postsLikedByThisUser.Contains(post.PostId);
                 post.IsBookmarked = postsSavedByThisUser.Contains(post.PostId);
             }

             return posts;
         }*/
        public async Task<PostDto[]> GetPostsAsync(int startIndex, int pageSize, Guid currentUserId)
        {
            var posts = await _dataContext.Set<PostDto>()
                .FromSqlInterpolated($"EXEC GetPosts @StartIndex={startIndex}, @PageSize={pageSize}, @CurrentUserId = {currentUserId}")
                .ToArrayAsync();

            return posts;
        }
        public async Task<PostDto?> GetPostAsync(Guid postId , Guid currentUserId)
        {
            var posts = await _dataContext.Set<PostDto>()
                .FromSqlInterpolated($"EXEC GetPostById  @PostId={postId}, @CurrentUserId = {currentUserId}")
                .ToArrayAsync();

            if (posts.Length == 0)
            {
                return null;
            }
            return posts[0];
        }
        //#19 Lưu bình luận cho bài đăng
        public async Task<ApiResult<CommentDto>> SaveCommentAsync(SaveCommentDto dto, LoggedInUser currentUser)
        {
            // Kiểm tra xem bài viết có tồn tại không
            var postExistsOwnerId = await _dataContext.Posts
                .Where(p => p.Id == dto.PostId  )
                .Select(p => p.UserId)
                .FirstOrDefaultAsync();
            if (postExistsOwnerId == default)
                return ApiResult<CommentDto>.Fail("Post not found");

            Comment? comment = null;
            bool sendNotification = false;
            // Kiểm tra xem đây là comment mới hay comment đã tồn tại
            if (dto.CommentId == Guid.Empty)
            {
                // Tạo mới comment
                comment = new Comment
                {
                    PostId = dto.PostId,
                    UserId = currentUser.ID,
                    Content = dto.Content,
                    AddedOn = DateTime.UtcNow
                };

                // Thêm comment mới vào DbContext
                _dataContext.Comments.Add(comment);
                sendNotification = true;
            }
            else
            {
                // Tìm comment đã tồn tại trong database
                comment = await _dataContext.Comments.FindAsync(dto.CommentId);

                // Kiểm tra xem comment có tồn tại không
                if (comment == null)
                {
                    return ApiResult<CommentDto>.Fail("Comment not found");
                }

                // Kiểm tra xem người dùng hiện tại có quyền chỉnh sửa comment không
                if (comment.UserId != currentUser.ID)
                {
                    return ApiResult<CommentDto>.Fail("You do not have permission to edit this comment");
                }

                // Cập nhật nội dung và thời gian chỉnh sửa của comment
                comment.Content = dto.Content;


                // Đánh dấu comment là đã được chỉnh sửa trong DbContext
                _dataContext.Comments.Update(comment);
            }

            try
            {
                // Lưu thay đổi vào database
                await _dataContext.SaveChangesAsync();

                // Tạo DTO để trả về thông tin comment
                var commentDto = new CommentDto
                {
                    CommentId = comment.Id,
                    Content = comment.Content,
                    AddedOn = comment.AddedOn,
                    UserId = currentUser.ID,
                    UserName = currentUser.Name,
                    UserPhotoUrl = currentUser.PhotoUrl
                };
                if (sendNotification)
                {
                    var notification = new NotificationDto
                    (
                        postExistsOwnerId,
                        $"{currentUser.Name} commented on your post",
                        DateTime.Now,
                        dto.PostId,
                        currentUser.PhotoUrl
                    );
                    await SaveNotificationAsync(notification);
                    await _hubContext.Clients.All.PostCommentAdded(commentDto);
                }
                // Trả về kết quả thành công cùng với thông tin comment
                return ApiResult<CommentDto>.Success(commentDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Xử lý ngoại lệ khi có xung đột cập nhật
                return ApiResult<CommentDto>.Fail("The comment has been edited by someone else. Please reload and try again.");
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ khác
                return ApiResult<CommentDto>.Fail($"An error occurred: {ex.Message}");
            }
        }
        //#19 Truy xuất/hiển thị bình luận cho bài đăng
        public async Task<CommentDto[]> GetCommentsAsync(Guid postId, int startIndex, int pageSize)
        {
            // Truy vấn danh sách bình luận theo postId từ cơ sở dữ liệu
            var comments = await _dataContext.Comments
                .Where(c => c.PostId == postId)  // Lọc các bình luận thuộc bài viết có postId tương ứng
                .OrderByDescending(c => c.AddedOn) // Sắp xếp bình luận theo thời gian giảm dần (mới nhất trước)
                .Skip(startIndex)
                .Take(pageSize)
                .Select(c => new CommentDto  // Chuyển đổi dữ liệu từ Entity sang DTO (Data Transfer Object)
                {
                    CommentId = c.Id,  // ID của bình luận
                    Content = c.Content, // Nội dung của bình luận
                    AddedOn = c.AddedOn, // Thời gian bình luận được thêm vào
                    UserId = c.UserId, // ID của người dùng bình luận
                    UserName = c.User.Name, // Tên của người dùng bình luận
                    UserPhotoUrl = c.User.PhotoUrl // Ảnh đại diện của người dùng
                })
                .ToArrayAsync(); // Thực hiện truy vấn bất đồng bộ và chuyển kết quả thành mảng

            return comments; // Trả về danh sách bình luận dưới dạng mảng DTO
        }

        //#20 Vấn đề 1: Đặt comment line cho code

        public async Task<ApiResult> DeletePostAsync(Guid postId, LoggedInUser currentUser)
        {
            try
            {
                // Tìm bài viết theo postId
                var post = await _dataContext.Posts.FindAsync(postId);
                if (post is null)
                    return ApiResult.Fail("Post not found"); // Trả về lỗi nếu không tìm thấy bài viết

                // Kiểm tra xem người dùng hiện tại có phải chủ bài viết không
                if (post.UserId != currentUser.ID)
                    return ApiResult.Fail("You can delete your own posts only"); // Chỉ cho phép xóa bài viết của chính mình

                // Xóa bài viết khỏi cơ sở dữ liệu
                //_dataContext.Posts.Remove(post);
                SafeDeleteFile(post.PhotoPath); // Xóa ảnh liên quan đến bài viết
                post.IsDeleted = true;
                _dataContext.Posts.Update(post);
                await _dataContext.SaveChangesAsync(); // Lưu thay đổi
                await _hubContext.Clients.All.PostDelete(postId);
                return ApiResult.Success(); // Trả về kết quả thành công
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex.Message); // Xử lý lỗi và trả về thông báo lỗi
            }
        }

        public async Task<ApiResult> ToggleBookmarkAsync(Guid postId, LoggedInUser currentUser)
        {
            // Kiểm tra xem bài viết có tồn tại không
            var postExistsOwnerId = await _dataContext.Posts
                .Where(p => p.Id == postId)
                .Select(p => p.UserId)
                .FirstOrDefaultAsync();
            if (postExistsOwnerId == default)
                return ApiResult.Fail("Post not found");

            try
            {
                bool sendNotification = false;
                // Kiểm tra xem bài viết đã được đánh dấu yêu thích chưa
                var bookmark = await _dataContext.Bookmarks.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == currentUser.ID);
                if (bookmark is null)
                {
                    // Nếu chưa, tạo mới bookmark
                    bookmark = new Bookmarks
                    {
                        PostId = postId,
                        UserId = currentUser.ID
                    };
                    _dataContext.Bookmarks.Add(bookmark);
                    sendNotification = true;
                }
                else
                {
                    // Nếu đã tồn tại, gỡ bỏ bookmark
                    _dataContext.Bookmarks.Remove(bookmark);
                }
                await _dataContext.SaveChangesAsync(); // Lưu thay đổi
                if (sendNotification)
                {
                    var notificationDto = new NotificationDto
                (
                    postExistsOwnerId,
                    $"{currentUser.Name} saved your post",
                    DateTime.Now,
                    postId,
                    currentUser.PhotoUrl

                    );
                    await SaveNotificationAsync(notificationDto);
                    await _hubContext.Clients.All.NotificationGenerated(notificationDto);
                }
                return ApiResult.Success(); // Trả về kết quả thành công
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex.Message); // Xử lý lỗi và trả về thông báo lỗi
            }
        }

        public async Task<ApiResult> ToggleLikeAsync(Guid postId, LoggedInUser currentUser)
        {
            // Kiểm tra xem bài viết có tồn tại không
            var postExistsOwnerId = await _dataContext.Posts
                .Where(p => p.Id == postId)
                .Select(p => p.UserId)
                .FirstOrDefaultAsync();
            if (postExistsOwnerId == default)
                return ApiResult.Fail("Post not found");

            try
            {
                bool sendNotification = false;
                // Kiểm tra xem bài viết đã được like chưa
                var like = await _dataContext.Likes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == currentUser.ID);
                if (like is null)
                {
                    // Nếu chưa, tạo mới like
                    like = new Likes
                    {
                        PostId = postId,
                        UserId = currentUser.ID
                    };
                    _dataContext.Likes.Add(like);
                }
                else
                {
                    // Nếu đã tồn tại, gỡ bỏ like
                    _dataContext.Likes.Remove(like);
                }
                await _dataContext.SaveChangesAsync(); // Lưu thay đổi
                if (sendNotification)
                { var notificationDto = new NotificationDto
                (
                    postExistsOwnerId,
                    $"{currentUser.Name} liked your post",
                    DateTime.Now,
                    postId,
                    currentUser.PhotoUrl

                    );
                    await SaveNotificationAsync(notificationDto);
                    await _hubContext.Clients.All.NotificationGenerated(notificationDto);
                }
                return ApiResult.Success(); // Trả về kết quả thành công
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex.Message); // Xử lý lỗi và trả về thông báo lỗi
            }
        }

        // Vấn đề 2: Prompt tạo code chính xác
        // "Viết hàm API trong C# sử dụng Entity Framework Core để thực hiện các chức năng sau: 
        // 1. Xóa bài viết (DeletePostAsync) với kiểm tra quyền sở hữu
        // 2. Toggle trạng thái Bookmark (ToggleBookmarkAsync)
        // 3. Toggle trạng thái Like (ToggleLikeAsync) 
        // Các hàm cần trả về ApiResult và xử lý lỗi một cách hợp lý."

        // Vấn đề 3: Kế hoạch hóa tư duy
        // 1. Xác định yêu cầu: Cần xử lý các thao tác trên bài viết (xóa, bookmark, like)
        // 2. Thiết kế cơ sở dữ liệu: Các bảng Posts, Bookmarks, Likes
        // 3. Viết code với Entity Framework để truy vấn và thay đổi dữ liệu
        // 4. Kiểm tra điều kiện (bài viết tồn tại, quyền sở hữu)
        // 5. Xử lý lỗi và đảm bảo API trả về phản hồi hợp lý
        // 6. Viết prompt mô tả chính xác yêu cầu để AI có thể tạo ra code đúng ý
        private async Task SaveNotificationAsync(NotificationDto dto)
        {
            try
            {
                // Kiểm tra người dùng có tồn tại không
                var userExists = await _dataContext.Users.AnyAsync(u => u.Id == dto.ForUserId);
                if (!userExists)
                {
                    throw new Exception("User not found");
                }

                // Kiểm tra bài viết có tồn tại không
                var postExists = await _dataContext.Posts.AnyAsync(p => p.Id == dto.PostId);
                if (!postExists)
                {
                    throw new Exception("Post not found");
                }

                var notification = new Notification
                {
                    ForUserId = dto.ForUserId,
                    PostId = dto.PostId,
                    Text = dto.Text,
                    When = dto.When
                };

                _dataContext.Notifications.Add(notification);
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving notification");
                throw;
            }
        }

    }


}