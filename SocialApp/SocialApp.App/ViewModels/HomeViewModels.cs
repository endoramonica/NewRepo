
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refit;
using SocialApp.App.Apis;
using SocialApp.App.Models;
using SocialApp.App.Pages;
using SocialApp.App.Services;
using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.IHub;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApp.App.ViewModels
{
    public partial class HomeViewModels : PostBaseViewModel
    {
        private readonly RealTimeUpdatesService _realTimeUpdatesService;
        private readonly AuthService _authService;
        

        public ObservableCollection<PostModel> Posts { get; set; } = [];
        

        public HomeViewModels(IPostApi postApi, IFollowApi followApi, IUserApi userApi, RealTimeUpdatesService realTimeUpdatesService, AuthService authService)
            : base(postApi)
        {
            _realTimeUpdatesService = realTimeUpdatesService;
            _authService = authService;
           
            FetchPostAsync();
            ConfigureRealTimeUpdates();
        }
        private int _startIndex = 0;
        private const int PageSize = 10;


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
                        //this is pull to refresh case 
                        //resey the post observable collection 
                        Posts.Clear();

                    }
                    _startIndex += posts.Length;
                    foreach (var p in posts)
                    {
                        Posts.Add(PostModel.FromDto(p));
                    }

                }



            });


        }
        [ObservableProperty]
        private bool _isRefreshView;
        [RelayCommand]
        private async Task RefreshPostAsync()
        {
            try
            {

                _startIndex = 0; // Reset the start index to fetch from the beginning
                await FetchPostAsync(); // Reuse the existing FetchPostAsync method
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🚨 [RefreshPostAsync] Error during refresh: {ex.Message}");
                await ShowErrorAlertAsync("Unable to refresh posts.");
            }
            finally
            {
                IsRefreshView = false; // Reset the refresh state
            }
        }
        [ObservableProperty]
        private bool _isThereNewNotification;
        public void ConfigureRealTimeUpdates()
        {
            _realTimeUpdatesService.AddPostChangeAction(nameof(HomeViewModels), OnPostChange);
            _realTimeUpdatesService.AddPostDeleteAction(nameof(HomeViewModels), OnPostDeleted);
            //_realTimeUpdatesService.AddPostLikeAction(nameof(HomeViewModels), PostLiked);
            //_realTimeUpdatesService.AddPostUnLikeAction(nameof(HomeViewModels), PostUnLiked);
            //_realTimeUpdatesService.AddPostBookmarkAction(nameof(HomeViewModels), PostBookmarked);
            //_realTimeUpdatesService.AddPostUnBookmarkAction(nameof(HomeViewModels), PostUnBookmarked);
            //_realTimeUpdatesService.AddCommentAddedAction(nameof(HomeViewModels), CommentAdded);
            _realTimeUpdatesService.AddUserPhotoChangeAction(nameof(HomeViewModels), OnUserPhotoChanged);
            _realTimeUpdatesService.AddNotificationGeneratedAction(nameof(HomeViewModels), OnNotificationGenerated);
        }
        private void OnNotificationGenerated(NotificationDto notification)
        {
            Debug.WriteLine($"[OnNotificationGenerated] Received notification for UserId: {notification.ForUserId}, Current UserId: {_authService.User.ID}");
            if (notification.ForUserId == _authService.User.ID)
            {
                Debug.WriteLine("[OnNotificationGenerated] Setting IsThereNewNotification to true");
                IsThereNewNotification = true;
            }
            else
            {
                Debug.WriteLine("[OnNotificationGenerated] User ID mismatch, not updating IsThereNewNotification");
            }
        }
        #region Methods

        /// <summary>
        /// Điều hướng đến trang thông báo.
        /// </summary>
        /// <returns>Task hoàn thành khi điều hướng xong.</returns>
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

        /// <summary>
        /// Điều hướng đến trang tạo bài viết.
        /// </summary>
        /// <returns>Task hoàn thành khi điều hướng xong.</returns>
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
                Debug.WriteLine($"🚨 [NavigateToCreatePostAsync] Lỗi khi điều hướng: {ex.Message}");
                await ShowErrorAlertAsync("Không thể điều hướng đến trang tạo bài viết.");
            }
        }

        /// <summary>
        /// Điều hướng đến trang cá nhân và hiển thị thông báo.
        /// </summary>
        /// <returns>Task hoàn thành khi điều hướng xong.</returns>
        [RelayCommand]
        private async Task NavigateToProfileAsync()
        {
            try
            {
                await NavigationAsync("//Profile");
                Debug.WriteLine("➡️ [NavigateToProfileAsync] Điều hướng đến Profile với userId: {targetUserId}");
                await ToastAsync("Chuyển đến trang cá nhân!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🚨 [NavigateToProfileAsync] Lỗi khi điều hướng: {ex.Message}");
                await ShowErrorAlertAsync("Không thể điều hướng đến trang cá nhân.");
            }
        }
        [RelayCommand]
        private async Task NavigateToFriendAsync()
        {
            try
            {
                await NavigationAsync("//FriendsPage");
                Debug.WriteLine("➡️ [NavigateToFriendAsync] Điều hướng đến Friend");
                await ToastAsync("Chuyển đến trang friend!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🚨 [NavigateToFriendAsync] Lỗi khi điều hướng: {ex.Message}");
                await ShowErrorAlertAsync("Không thể điều hướng đến trang friend.");
            }
        }

        /// <summary>
        /// Điều hướng đến trang chi tiết bài viết.
        /// </summary>
        /// <returns>Task hoàn thành khi điều hướng xong.</returns>
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
                Debug.WriteLine($"🚨 [NavigateToPostDetailAsync] Lỗi khi điều hướng: {ex.Message}");
                await ShowErrorAlertAsync("Không thể điều hướng đến trang chi tiết bài viết.");
            }
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
        //private void OnPostLiked(Guid postId, Guid userId)
        //{
        //    var post = Posts.FirstOrDefault(p => p.PostId == postId);
        //    post?.LikedUserIds.Add(userId);
        //}

        //private void OnPostUnLiked(Guid postId, Guid userId)
        //{
        //    var post = Posts.FirstOrDefault(p => p.PostId == postId);
        //    post?.LikedUserIds.Remove(userId);
        //}

        //private void OnPostBookmarked(Guid postId, Guid userId)
        //{
        //    var post = Posts.FirstOrDefault(p => p.PostId == postId);
        //    post?.BookmarkedUserIds.Add(userId);
        //}

        //private void OnPostUnBookmarked(Guid postId, Guid userId)
        //{
        //    var post = Posts.FirstOrDefault(p => p.PostId == postId);
        //    post?.BookmarkedUserIds.Remove(userId);
        //}

        //private void OnCommentAdded(CommentDto comment)
        //{
        //    var post = Posts.FirstOrDefault(p => p.PostId == comment.PostId);
        //    post?.Comments.Add(comment); // giả sử post.Comments là một danh sách
        //}

        private void OnUserPhotoChanged(UserPhotoChange change)
        {
            foreach (var post in Posts.Where(p => p.UserId == change.UserId))
            {
                post.UserPhotoUrl = change.UserPhotoUrl;
            }
        }

        //private void OnNotificationGenerated(NotificationDto notification)
        //=> IsThereNewNotification = true;
        //// Ghi lại thông báo vào danh sách thông báo của người dùng
        //// Hoặc thực hiện các hành động khác tùy theo yêu cầu


        #endregion
    }
}