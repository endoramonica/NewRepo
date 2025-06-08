using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refit;
using SocialApp.App.Apis;
using SocialApp.App.Models;
using SocialApp.App.Services;
using SocialAppLibrary.Shared.Dtos.ChatDto;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SocialApp.App.ViewModels;

public partial class FriendViewModel : PostBaseViewModel, IDisposable
{
    private readonly IUserApi _userApi;
    private readonly AuthService _authService;
    private readonly RealTimeUpdatesService _realTimeUpdatesService;

    [ObservableProperty]
    private ObservableCollection<UserDto> _friends = [];

    [ObservableProperty]
    private ObservableCollection<UserDto> _pendingRequests = [];

    [ObservableProperty]
    private string _toUserIdInput;

    [ObservableProperty]
    private Guid _currentUserId;

    public bool HasFriends => Friends?.Count > 0;
    public bool HasNoFriends => !HasFriends;

    public FriendViewModel(
        IUserApi userApi,
        AuthService authService,
        RealTimeUpdatesService realTimeUpdatesService,
        IPostApi postApi)
        : base(postApi)
    {
        _userApi = userApi ?? throw new ArgumentNullException(nameof(userApi));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _realTimeUpdatesService = realTimeUpdatesService ?? throw new ArgumentNullException(nameof(realTimeUpdatesService));

        _currentUserId = _authService.User?.ID ?? throw new InvalidOperationException("User must be logged in.");

        ConfigureRealTimeUpdates();
    }

    [RelayCommand]
    public async Task LoadFriendsAsync()
    {
        await MakeApiCall(async () =>
        {
            var result = await _userApi.GetUserFriendsAsync(_currentUserId);
            if (result.IsSuccess)
            {
                Friends.Clear();
                foreach (var friend in result.Data)
                    Friends.Add(friend);
                OnPropertyChanged(nameof(HasFriends));
                OnPropertyChanged(nameof(HasNoFriends));
            }
            else
            {
                await ShowErrorAlertAsync(result.Error);
            }
        });
    }

    [RelayCommand]
    public async Task LoadPendingRequestsAsync()
    {
        await MakeApiCall(async () =>
        {
            var result = await _userApi.GetPendingFriendRequestsAsync(_currentUserId);
            if (result.IsSuccess)
            {
                PendingRequests.Clear();
                foreach (var request in result.Data)
                    PendingRequests.Add(request);
            }
            else
            {
                await ShowErrorAlertAsync(result.Error);
            }
        });
    }

    [RelayCommand]
    private async Task SendFriendRequestAsync()
    {
        // Xác thực ToUserIdInput
        if (string.IsNullOrWhiteSpace(_toUserIdInput) || !Guid.TryParse(_toUserIdInput, out var toUserId))
        {
            await ToastAsync("Lỗi: Vui lòng nhập ID người dùng hợp lệ!");
            return;
        }

        var dto = new FriendRequestDto(_currentUserId, toUserId);

        if (dto.FromUserId == Guid.Empty || dto.ToUserId == Guid.Empty)
        {
            await ToastAsync("Lỗi: ID người dùng không hợp lệ!");
            return;
        }
        if (dto.FromUserId == dto.ToUserId)
        {
            await ToastAsync("Lỗi: Không thể gửi lời mời kết bạn cho chính mình!");
            return;
        }
        if (dto.FromUserId != _currentUserId)
        {
            await ToastAsync("Lỗi: ID người gửi không khớp với người dùng hiện tại!");
            return;
        }

        await MakeApiCall(async () =>
        {
            try
            {
                Console.WriteLine($"[DEBUG] Sending friend request: FromUserId={dto.FromUserId}, ToUserId={dto.ToUserId}");
                var result = await _userApi.SendFriendRequestAsync(dto);
                if (result.IsSuccess)
                {
                    await ToastAsync("Đã gửi lời mời kết bạn!");
                    ToUserIdInput = "";
                }
                else
                {
                    await ToastAsync($"Lỗi: {result.Error}");
                }
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"[DEBUG] API Error in SendFriendRequestAsync: StatusCode={ex.StatusCode}, Message={ex.Message}, Content={ex.Content}");
                throw;
            }
        });
    }

    [RelayCommand]
    private async Task AcceptFriendRequestAsync(Guid userId)
    {
        var dto = new FriendActionDto(_currentUserId, userId);
        await MakeApiCall(async () =>
        {
            var result = await _userApi.AcceptFriendRequestAsync(dto);
            if (result.IsSuccess)
            {
                await LoadPendingRequestsAsync();
                await LoadFriendsAsync();
            }
            await ToastAsync(result.IsSuccess ? "Đã chấp nhận lời mời!" : $"Lỗi: {result.Error}");
        });
    }

    [RelayCommand]
    private async Task MessageFriendAsync(UserDto friend)
    {
        await ToastAsync($"Bắt đầu nhắn tin với {friend.Name}");
    }

    [RelayCommand]
    private async Task RejectFriendRequestAsync(Guid userId)
    {
        var dto = new FriendActionDto(_currentUserId, userId);
        await MakeApiCall(async () =>
        {
            var result = await _userApi.RejectFriendRequestAsync(dto);
            if (result.IsSuccess)
                await LoadPendingRequestsAsync();
            await ToastAsync(result.IsSuccess ? "Đã từ chối lời mời!" : $"Lỗi: {result.Error}");
        });
    }

    [RelayCommand]
    private async Task RemoveFriendAsync(Guid friendId)
    {
        var dto = new FriendActionDto(_currentUserId, friendId);
        await MakeApiCall(async () =>
        {
            var result = await _userApi.RemoveFriendAsync(dto);
            if (result.IsSuccess)
                await LoadFriendsAsync();
            await ToastAsync(result.IsSuccess ? "Đã xóa bạn bè!" : $"Lỗi: {result.Error}");
        });
    }

    public void ConfigureRealTimeUpdates()
    {
        _realTimeUpdatesService.AddFriendRequestSentAction(nameof(FriendViewModel), async notification =>
        {
            await ToastAsync($"{notification.FromUserName} đã gửi lời mời kết bạn!");
            await LoadPendingRequestsAsync();
        });

        _realTimeUpdatesService.AddFriendRequestAcceptedAction(nameof(FriendViewModel), async notification =>
        {
            await ToastAsync($"{notification.FromUserName} đã chấp nhận lời mời của bạn!");
            await LoadFriendsAsync();
        });

        _realTimeUpdatesService.AddFriendRequestRejectedAction(nameof(FriendViewModel), async notification =>
        {
            await ToastAsync(" đã từ chối lời mời của bạn.");
        });

        _realTimeUpdatesService.AddFriendRemovedAction(nameof(FriendViewModel), async notification =>
        {
            await ToastAsync($"{notification.FromUserName} đã xóa bạn khỏi danh sách bạn bè.");
            await LoadFriendsAsync();
        });

        Console.WriteLine("[DEBUG] Real-time updates configured for FriendViewModel");
    }

    public void Dispose()
    {
        _realTimeUpdatesService?.RemoveHandler(nameof(FriendViewModel));
        Console.WriteLine("[DEBUG] FriendViewModel disposed");
    }
}