using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SocialApp.App.Apis;
using SocialApp.App.Models;
using SocialApp.App.Services;
using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.IHub;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SocialApp.App.ViewModels
{
    public partial class HomeViewModels : PostBaseViewModel
    {
        #region Constructor & Init

        private readonly RealTimeUpdatesService _realTimeUpdatesService;
        private readonly AuthService _authService;

        public HomeViewModels(IPostApi postApi, RealTimeUpdatesService realTimeUpdatesService, AuthService authService)
            : base(postApi)
        {
            FetchPostAsync();
            _realTimeUpdatesService = realTimeUpdatesService;
            _authService = authService;
           
        }

        #endregion

        #region Properties

        public ObservableCollection<PostModel> Posts { get; set; } = [];

        private int _startIndex = 0;
        private const int PageSize = 10;

        [ObservableProperty]
        private bool _isRefreshView;

        [ObservableProperty]
        private bool _isThereNewNotification;

        #endregion

        #region Fetch/Refresh

        [RelayCommand]
        private async Task FetchPostAsync()
        {
            await MakeApiCall(async () =>
            {
                var posts = await PostApi.GetPostsAsync(_startIndex, PageSize);
                if (posts.Length > 0)
                {
                    if (_startIndex == 0 && Posts.Count > 0)
                    {
                        Posts.Clear(); // Đây là trường hợp pull-to-refresh
                    }

                    _startIndex += posts.Length;

                    foreach (var p in posts)
                    {
                        Posts.Add(PostModel.FromDto(p));
                    }
                }
            });
        }

        [RelayCommand]
        private async Task RefreshPostAsync()
        {
            try
            {
                _startIndex = 0; // Reset lại để fetch từ đầu
                await FetchPostAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🚨 [RefreshPostAsync] Lỗi khi refresh: {ex.Message}");
                await ShowErrorAlertAsync("Không thể làm mới bài viết.");
            }
            finally
            {
                IsRefreshView = false; // Reset trạng thái pull-to-refresh
            }
        }

        #endregion

        #region Navigation

        [RelayCommand]
        private async Task NavigateToNotificationAsync()
        {
            try
            {
                await NavigationAsync("//NotificationPage");
            }
            catch (Exception ex)
            {
                await ShowErrorAlertAsync("Không thể điều hướng đến trang thông báo.");
            }
        }

        [RelayCommand]
        private async Task NavigateToCreatePostAsync()
        {
            try
            {
                await NavigationAsync("///CreatePostPage");
                Debug.WriteLine("➡️ [NavigateToCreatePostAsync] Điều hướng đến CreatePostPage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🚨 [NavigateToCreatePostAsync] Lỗi: {ex.Message}");
                await ShowErrorAlertAsync("Không thể điều hướng đến trang tạo bài viết.");
            }
        }

        [RelayCommand]
        private async Task NavigateToProfileAsync()
        {
            try
            {
                await NavigationAsync("//Profile");
                Debug.WriteLine("➡️ [NavigateToProfileAsync] Điều hướng đến Profile");
                await ToastAsync("Chuyển đến trang cá nhân!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🚨 [NavigateToProfileAsync] Lỗi: {ex.Message}");
                await ShowErrorAlertAsync("Không thể điều hướng đến trang cá nhân.");
            }
        }

        [RelayCommand]
        private async Task NavigateToPostDetailAsync()
        {
            try
            {
                await NavigationAsync("///PostDetailsPage");
                Debug.WriteLine("➡️ [NavigateToPostDetailAsync] Điều hướng đến PostDetailsPage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🚨 [NavigateToPostDetailAsync] Lỗi: {ex.Message}");
                await ShowErrorAlertAsync("Không thể điều hướng đến trang chi tiết bài viết.");
            }
        }

        #endregion

        #region Real-time Events

        public void ConfigureRealTimeUpdates()
        {
            _realTimeUpdatesService.AddPostChangeAction(nameof(HomeViewModels), OnPostChange);
            _realTimeUpdatesService.AddPostDeleteAction(nameof(HomeViewModels), OnPostDeleted);
            _realTimeUpdatesService.AddUserPhotoChangeAction(nameof(HomeViewModels), OnUserPhotoChanged);
            _realTimeUpdatesService.AddNotificationGeneratedAction(nameof(HomeViewModels), OnNotificationGenerated);
        }

        private void OnPostChange(PostDto post)
        {
            var currentPost = Posts.FirstOrDefault(p => p.PostId == post.PostId);
            if (currentPost != null)
            {
                currentPost.Content = post.Content;
                currentPost.PhotoUrl = post.PhotoUrl;
            }
            else
            {
                Posts.Insert(0, PostModel.FromDto(post));
            }
        }

        private void OnPostDeleted(Guid postId)
        {
            var post = Posts.FirstOrDefault(p => p.PostId == postId);
            if (post != null)
            {
                Posts.Remove(post);
                _startIndex--;
            }
        }

        private void OnUserPhotoChanged(UserPhotoChange change)
        {
            foreach (var post in Posts.Where(p => p.UserId == change.UserId))
            {
                post.UserPhotoUrl = change.UserPhotoUrl;
            }
        }

        private void OnNotificationGenerated(NotificationDto notification)
        {
            if (notification.ForUserId == _authService.User.ID)
            {
                IsThereNewNotification = true;
            }
        }

        #endregion

        #region Helpers

        private async Task NavigationAsync(string route)
        {
            // Logic điều hướng nếu có thêm xử lý
        }

        private async Task ShowErrorAlertAsync(string message)
        {
            // Logic hiển thị cảnh báo
        }

        private async Task ToastAsync(string message)
        {
            // Logic hiện thông báo nhanh
        }

        #endregion
    }
}
