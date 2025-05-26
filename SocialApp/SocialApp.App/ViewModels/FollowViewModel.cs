using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SocialApp.App.Apis;
using SocialApp.App.Services;
using SocialAppLibrary.Shared.Dtos;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace SocialApp.App.ViewModels
{
    public partial class FollowViewModel : PostBaseViewModel
    {
        private readonly IUserApi _userApi;
        private readonly IFollowApi _followApi;
        private readonly AuthService _authService;
        private readonly RealTimeUpdatesService _realTimeUpdatesService;
        private int _startIndex = 0;
        private const int PageSize = 10;

        public FollowViewModel(IPostApi postApi,IFollowApi followApi, IUserApi userApi, AuthService authService, RealTimeUpdatesService realTimeUpdatesService)
            : base(postApi)
        {
            _userApi = userApi;
            _followApi = followApi;
            _authService = authService;
            _realTimeUpdatesService = realTimeUpdatesService;
            ConfigureRealTimeUpdates();
        }

        [ObservableProperty]
        private ObservableCollection<LoggedInUser> _followers = new();

        [ObservableProperty]
        private ObservableCollection<LoggedInUser> _following = new();

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private bool _isFollowingTab = true;

        //public object SearchFollowersAsyncCommand { get;  set; }

        [RelayCommand]
        private async Task LoadFollowersAsync()
        {
            await MakeApiCall(async () =>
            {
                var result = await _followApi.GetFollowersAsync(_startIndex, PageSize);
                if (result.IsSuccess)
                {
                    Followers.Clear();
                    foreach (var follower in result.Data ?? [])
                    {
                        Followers.Add(follower);
                    }
                    _startIndex += PageSize;
                }
                else
                {
                    await ShowErrorAlertAsync(result.Error ?? "Failed to load followers.");
                }
            });
        }

        [RelayCommand]
        private async Task LoadFollowingAsync()
        {
            await MakeApiCall(async () =>
            {
                var result = await _followApi.GetFollowingAsync(_startIndex, PageSize);
                if (result.IsSuccess)
                {
                    Following.Clear();
                    foreach (var user in result.Data ?? [])
                    {
                        Following.Add(user);
                    }
                    _startIndex += PageSize;
                }
                else
                {
                    await ShowErrorAlertAsync(result.Error ?? "Failed to load following.");
                }
            });
        }

        [RelayCommand]
        private async Task SearchFollowersAsync()
        {
            await MakeApiCall(async () =>
            {
                var result = await _followApi.SearchFollowersAsync(_searchQuery, _startIndex, PageSize);
                if (result.IsSuccess)
                {
                    Followers.Clear();
                    var followersData = result.Data ?? [];
                    if (followersData.Length == 0)
                    {
                        await ToastAsync("Không tìm thấy người dùng nào.");
                    }
                    else
                    {
                        foreach (var follower in followersData)
                        {
                            Followers.Add(follower);
                        }
                    }
                }
                else
                {
                    await ShowErrorAlertAsync(result.Error ?? "Failed to search followers.");
                }
            });
        }

        partial void OnSearchQueryChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _startIndex = 0; // Reset startIndex khi tìm kiếm mới
               // SearchFollowersAsyncCommand.Execute(null);
            }
        }

        [RelayCommand]
        private async Task FollowAsync(Guid followingId)
        {
            await MakeApiCall(async () =>
            {
                var result = await _followApi.FollowAsync(followingId);
                if (result.IsSuccess)
                {
                    await ToastAsync(result.Data ?? "Followed successfully.");
                    await LoadFollowersAsync(); // Cập nhật danh sách sau khi follow
                }
                else
                {
                    await ShowErrorAlertAsync(result.Error ?? "Failed to follow.");
                }
            });
        }

        [RelayCommand]
        private async Task UnfollowAsync(Guid followingId)
        {
            await MakeApiCall(async () =>
            {
                var result = await _followApi.UnfollowAsync(followingId);
                if (result.IsSuccess)
                {
                    await ToastAsync(result.Data ?? "Unfollowed successfully.");
                    await LoadFollowersAsync(); // Cập nhật danh sách sau khi unfollow
                }
                else
                {
                    await ShowErrorAlertAsync(result.Error ?? "Failed to unfollow.");
                }
            });
        }

        [RelayCommand]
        private void SwitchTab(bool isFollowingTab)
        {
            IsFollowingTab = isFollowingTab;
            _startIndex = 0; // Reset index khi chuyển tab
            if (isFollowingTab)
            {
                LoadFollowingAsync();
            }
            else
            {
                LoadFollowersAsync();
            }
        }

        public void ConfigureRealTimeUpdates()
        {
            _realTimeUpdatesService.AddFollowNotificationAction(nameof(FollowViewModel), notification =>
            {
                OnFollowNotification(notification);
            });

        }

        private async void OnFollowNotification(FollowNotificationDto notification)
        {
            // Kiểm tra xem thông báo có liên quan đến người dùng hiện tại không
            if (notification.FollowerId != _authService.User.ID)
            {
                await Shell.Current.Dispatcher.DispatchAsync(() =>
                {
                    // Sử dụng thông tin Follower từ notification
                    var newFollower = notification.Follower;
                    if (newFollower != null && !Followers.Any(f => f.ID == newFollower.ID))
                    {
                        Followers.Insert(0, newFollower); // Thêm vào đầu danh sách
                        ToastAsync(notification.Message); // Hiển thị thông báo
                    }
                });
            }
        }
    }
}