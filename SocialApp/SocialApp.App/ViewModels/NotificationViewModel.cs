using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SocialApp.App.Apis;
using SocialApp.App.Models;
using SocialApp.App.Services;
using SocialAppLibrary.Shared.Dtos;
using System.Collections.ObjectModel;

namespace SocialApp.App.ViewModels;

public partial class NotificationViewModel : PostBaseViewModel
{
    #region Fields

    private readonly IUserApi _userApi;
    private readonly RealTimeUpdatesService _realTimeUpdatesService;
    private readonly AuthService _authService;
    private readonly IPostApi _postApi;
    private int _startIndex = 0;
    private const int _pageSize = 50;

    #endregion

    #region Properties

    public ObservableCollection<NotificationDto> Notifications { get; set; } = [];

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private bool _isThereNewNotification;

    #endregion

    #region Constructor

    public NotificationViewModel
        (IUserApi userApi,
        RealTimeUpdatesService realTimeUpdatesService,
        AuthService authService,IPostApi postApi) :base(postApi)
    {
        _userApi = userApi;
        _realTimeUpdatesService = realTimeUpdatesService;
        _authService = authService;
        _postApi = postApi;
        FetchNotificationsAsync();
    }

    #endregion

    #region Public Methods

    [RelayCommand]
    public async Task FetchNotificationsAsync()
    {
        await MakeApiCall(async () =>
        {
            var notifications = await _userApi.GetNotificationsAsync(_startIndex, _pageSize);
            if (notifications != null && notifications.Length > 0)
            {
                if (_startIndex == 0 && Notifications.Count > 0)
                {
                    Notifications.Clear(); // Pull-to-refresh scenario
                }

                _startIndex += notifications.Length;

                foreach (var notification in notifications)
                {
                    Notifications.Add(notification);
                }
            }
        });
    }

    [RelayCommand]
    private async Task RefreshNotificationsAsync()
    {
        _startIndex = 0;
        await FetchNotificationsAsync();
        IsRefreshing = false;
    }
    [RelayCommand]
    private async Task OpenPostAsync(Guid? postId)
    {
        if (postId.HasValue && postId != default)
        {
            await MakeApiCall(async () =>
            {
                var post = await _postApi.GetPostAsync(postId.Value);
                if (post == null)
                {
                    await ToastAsync("Post not found");
                    return; 
                }
                GoToPostDetailsPageCommand.Execute(PostModel.FromDto(post));
            });
        }
        else
        {
            await Shell.Current.GoToAsync("//CreatePostPage", animate: true);
        }
    }
    #endregion

    #region Real-Time Updates

    public void ConfigureRealTimeUpdates()
    {
       
        _realTimeUpdatesService.AddNotificationGeneratedAction(nameof(NotificationViewModel), OnNotificationGenerated);
    }

    private async void OnNotificationGenerated(NotificationDto notification)
    {
        if (notification.ForUserId == _authService.User.ID)
        {
            await Shell.Current.Dispatcher.DispatchAsync(()=>  ToastAsync("You have a new notification"));
            Notifications = [notification, .. Notifications];
            OnPropertyChanged(nameof(Notifications));
        }
    }

   
    #endregion
}
