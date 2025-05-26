using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refit;
using SocialApp.App.Apis;
using SocialApp.App.Models;
using SocialApp.App.Pages;
using SocialApp.App.Services;
using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.IHub;
using System.Collections.ObjectModel;
using System.IO;

namespace SocialApp.App.ViewModels;

[QueryProperty(nameof(CropPhotoSource), "new-src")]
public partial class ProfileViewModel : PostBaseViewModel, IDisposable
{
    private readonly IFollowApi _followApi;
    private readonly AuthService _authService;
    private readonly IUserApi _userApi;
    private readonly RealTimeUpdatesService _realTimeUpdatesService;

    public FollowViewModel FollowViewModel { get; }

    public ProfileViewModel(IPostApi postApi, IFollowApi followApi, AuthService authService, IUserApi userApi, RealTimeUpdatesService realTimeUpdatesService)
        : base(postApi)
    {
        _followApi = followApi;
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _userApi = userApi;
        _realTimeUpdatesService = realTimeUpdatesService;
        User = _authService.User ?? throw new InvalidOperationException("User must not be null.");
        FollowViewModel = new FollowViewModel(postApi, followApi, userApi, authService, realTimeUpdatesService);
        Console.WriteLine($"[DEBUG] ProfileViewModel initialized. CropPhotoSource: {CropPhotoSource}");
    }

    [ObservableProperty]
    private UserModel _targetUser = new();

    [ObservableProperty]
    private bool _isOwner;

    async partial void OnTargetUserChanged(UserModel value)
    {
        IsOwner = _authService.User?.ID == value.ID;
    }

    public enum ProfileTab
    {
        MyPosts,
        Bookmarked,
        Liked
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayPhotoUrl))]
    [NotifyPropertyChangedFor(nameof(CurrentUserModel))]
    private LoggedInUser _user;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsMyPostTabSelected))]
    [NotifyPropertyChangedFor(nameof(IsBookMarkedTabSelected))]
    [NotifyPropertyChangedFor(nameof(IsLikedTabSelected))]
    private ProfileTab _selectedTab = ProfileTab.MyPosts;

    public bool IsMyPostTabSelected => SelectedTab == ProfileTab.MyPosts;
    public bool IsBookMarkedTabSelected => SelectedTab == ProfileTab.Bookmarked;
    public bool IsLikedTabSelected => SelectedTab == ProfileTab.Liked;

    public ObservableCollection<PostModel> MyPosts { get; set; } = [];
    public ObservableCollection<PostModel> BookMarkedPost { get; set; } = [];
    public ObservableCollection<PostModel> LikedPosts { get; set; } = [];

    private int _myPostStartIndex = 0;
    private int _bookMarkedPostStartIndex = 0;
    private int _likedPostStartIndex = 0;

    private const int PageSize = 4;

    public string DisplayPhotoUrl => string.IsNullOrWhiteSpace(User?.PhotoUrl) ? "add_a_photo.png" : User.PhotoUrl;

    public UserModel CurrentUserModel => User != null ? UserModel.FromLoggedInUser(User) : new UserModel();

    [RelayCommand]
    private async Task LogoutAsync()
    {
        if (await App.Current.MainPage.DisplayAlert("Đăng xuất", "Bạn có chắc chắn muốn đăng xuất?", "Có", "Không"))
        {
            _authService.Logout();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    [RelayCommand]
    private async Task FetchPostsAsync()
    {
        switch (SelectedTab)
        {
            case ProfileTab.MyPosts:
                await FetchMyPostsAsync();
                break;
            case ProfileTab.Bookmarked:
                await FetchBookmarkedPostsAsync();
                break;
            case ProfileTab.Liked:
                await FetchLikedPostsAsync();
                break;
        }
    }

    [RelayCommand]
    private async Task SelectMyPostTabAsync()
    {
        SelectedTab = ProfileTab.MyPosts;
        _myPostStartIndex = 0;
        MyPosts.Clear();
        await FetchMyPostsAsync();
    }

    [RelayCommand]
    private async Task SelectBookMarkedPostsTabAsync()
    {
        SelectedTab = ProfileTab.Bookmarked;
        _bookMarkedPostStartIndex = 0;
        BookMarkedPost.Clear();
        await FetchBookmarkedPostsAsync();
    }

    [RelayCommand]
    private async Task SelectLikedPostsTabAsync()
    {
        SelectedTab = ProfileTab.Liked;
        _likedPostStartIndex = 0;
        LikedPosts.Clear();
        await FetchLikedPostsAsync();
    }

    [RelayCommand]
    public async Task FetchMyPostsAsync()
    {
        await MakeApiCall(async () =>
        {
            var posts = await _userApi.GetUserPostsAsync(_myPostStartIndex, PageSize);
            if (posts.Length > 0)
            {
                _myPostStartIndex += posts.Length;
                foreach (var p in posts)
                {
                    var model = PostModel.FromDto(p);
                    if (model != null)
                        MyPosts.Add(model);
                }
            }
        });
    }

    [RelayCommand]
    private async Task FetchBookmarkedPostsAsync()
    {
        await MakeApiCall(async () =>
        {
            var posts = await _userApi.GetUserBookmarkedPostsAsync(_bookMarkedPostStartIndex, PageSize);
            if (posts.Length > 0)
            {
                _bookMarkedPostStartIndex += posts.Length;
                foreach (var p in posts)
                {
                    var model = PostModel.FromDto(p);
                    if (model != null)
                        BookMarkedPost.Add(model);
                }
            }
        });
    }

    [RelayCommand]
    private async Task FetchLikedPostsAsync()
    {
        await MakeApiCall(async () =>
        {
            var posts = await _userApi.GetUserLikedPostsAsync(_likedPostStartIndex, PageSize);
            if (posts.Length > 0)
            {
                _likedPostStartIndex += posts.Length;
                foreach (var p in posts)
                {
                    var model = PostModel.FromDto(p);
                    if (model != null)
                        LikedPosts.Add(model);
                }
            }
        });
    }

    protected override void OnToggleBookmarkedAsync(PostModel postModel)
    {
        var currentPost = BookMarkedPost.FirstOrDefault(p => p.PostId == postModel.PostId);
        if (currentPost != null && !postModel.IsBookmarked)
            BookMarkedPost.Remove(currentPost);
    }

    protected override void OnToggleLikedAsync(PostModel postModel)
    {
        var currentPost = LikedPosts.FirstOrDefault(p => p.PostId == postModel.PostId);
        if (currentPost != null && !postModel.IsLiked)
            LikedPosts.Remove(currentPost);
    }

    [RelayCommand]
    private async Task ChangePhotoAsync()
    {
        Console.WriteLine("[DEBUG] ChangePhotoAsync triggered");
        var selectedPhotoSource = await ChoosePhotoAsync();
        if (!string.IsNullOrWhiteSpace(selectedPhotoSource))
        {
            var param = new Dictionary<string, object>
            {
                [nameof(CropImagePage.PhotoSource)] = selectedPhotoSource
            };
            await Shell.Current.GoToAsync("//CropImagePage", param);
            Console.WriteLine($"[DEBUG] Navigating to CropImagePage with PhotoSource={selectedPhotoSource}");
        }
        else
        {
            Console.WriteLine("[DEBUG] No photo selected in ChangePhotoAsync");
        }
    }

    [ObservableProperty]
    private string? _cropPhotoSource;

    async partial void OnCropPhotoSourceChanged(string? oldValue, string? newValue)
    {
        Console.WriteLine($"[DEBUG] OnCropPhotoSourceChanged triggered. Old: {oldValue}, New: {newValue}");
        if (string.IsNullOrWhiteSpace(newValue))
        {
            Console.WriteLine("[DEBUG] CropPhotoSource is null or empty");
            return;
        }

        if (!File.Exists(newValue))
        {
            Console.WriteLine($"[ERROR] File does not exist: {newValue}");
            await ShowErrorAlertAsync("Tệp ảnh không tồn tại.");
            return;
        }

        await MakeApiCall(async () =>
        {
            try
            {
                Console.WriteLine($"[DEBUG] Attempting to read file: {newValue}");
                var photoName = Path.GetFileName(newValue);
                using var fs = File.OpenRead(newValue);
                var photoStreamPart = new StreamPart(fs, photoName);

                Console.WriteLine("[DEBUG] Calling ChangePhotoAsync");
                var result = await _userApi.ChangePhotoAsync(photoStreamPart);
                if (!result.IsSuccess)
                {
                    Console.WriteLine($"[ERROR] API call failed: {result.Error}");
                    await ShowErrorAlertAsync(result.Error);
                    return;
                }

                var newPhotoUrl = result.Data;
                Console.WriteLine($"[DEBUG] New photo URL: {newPhotoUrl}");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    User = User with { PhotoUrl = newPhotoUrl };
                    _authService.Login(new LoginResponse(User, _authService.Token));
                    UpdateUserPhotoInPosts(newPhotoUrl);
                    Console.WriteLine("[DEBUG] User photo updated in posts");

                    // Xóa tệp tạm sau khi API thành công
                    if (File.Exists(newValue))
                    {
                        File.Delete(newValue);
                        Console.WriteLine($"[DEBUG] Deleted temporary file: {newValue}");
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exception in OnCropPhotoSourceChanged: {ex.Message}");
                await ShowErrorAlertAsync(ex.Message);
            }
        });
    }

    private void UpdateUserPhotoInPosts(string newPhotoUrl)
    {
        var currentUserId = User.ID;
        Console.WriteLine($"[DEBUG] Updating user photo in posts. UserID: {currentUserId}, NewPhotoUrl: {newPhotoUrl}");

        foreach (var post in MyPosts.Where(p => p.UserId == currentUserId))
        {
            post.UserPhotoUrl = newPhotoUrl;
        }

        foreach (var post in BookMarkedPost.Where(p => p.UserId == currentUserId))
        {
            post.UserPhotoUrl = newPhotoUrl;
        }

        foreach (var post in LikedPosts.Where(p => p.UserId == currentUserId))
        {
            post.UserPhotoUrl = newPhotoUrl;
        }
    }

    #region Real-time Events

    public void ConfigureRealTimeUpdates()
    {
        _realTimeUpdatesService.AddPostChangeAction(nameof(ProfileViewModel), OnPostChange);
        _realTimeUpdatesService.AddPostDeleteAction(nameof(ProfileViewModel), OnPostDeleted);
        _realTimeUpdatesService.AddUserPhotoChangeAction(nameof(ProfileViewModel), OnUserPhotoChanged);
        Console.WriteLine("[DEBUG] Real-time updates configured");
    }

    private void UpdatePostInCollection(ObservableCollection<PostModel> collection, PostDto post)
    {
        var existing = collection.FirstOrDefault(p => p.PostId == post.PostId);
        if (existing != null)
        {
            existing.Content = post.Content;
            existing.IsBookmarked = post.IsBookmarked;
            existing.IsLiked = post.IsLiked;
            existing.UserPhotoUrl = post.UserPhotoUrl;
            Console.WriteLine($"[DEBUG] Updated post {post.PostId} in collection");
        }
    }

    private void OnPostChange(PostDto post)
    {
        UpdatePostInCollection(MyPosts, post);
        UpdatePostInCollection(BookMarkedPost, post);
        UpdatePostInCollection(LikedPosts, post);
    }

    private void OnPostDeleted(Guid postId)
    {
        var myPost = MyPosts.FirstOrDefault(p => p.PostId == postId);
        if (myPost != null)
        {
            MyPosts.Remove(myPost);
            Console.WriteLine($"[DEBUG] Removed post {postId} from MyPosts");
        }

        var bookmarked = BookMarkedPost.FirstOrDefault(p => p.PostId == postId);
        if (bookmarked != null)
        {
            BookMarkedPost.Remove(bookmarked);
            Console.WriteLine($"[DEBUG] Removed post {postId} from BookMarkedPost");
        }

        var liked = LikedPosts.FirstOrDefault(p => p.PostId == postId);
        if (liked != null)
        {
            LikedPosts.Remove(liked);
            Console.WriteLine($"[DEBUG] Removed post {postId} from LikedPosts");
        }
    }

    private void OnUserPhotoChanged(UserPhotoChange change)
    {
        Console.WriteLine($"[DEBUG] OnUserPhotoChanged triggered. UserId: {change.UserId}, NewPhotoUrl: {change.UserPhotoUrl}");
        if (change.UserId == User.ID)
        {
            User = User with { PhotoUrl = change.UserPhotoUrl };
            _authService.Login(new LoginResponse(User, _authService.Token));
            UpdateUserPhotoInPosts(change.UserPhotoUrl);
        }
        else
        {
            foreach (var post in BookMarkedPost.Where(post => post.UserId == change.UserId))
            {
                post.UserPhotoUrl = change.UserPhotoUrl;
            }
            foreach (var post in LikedPosts.Where(post => post.UserId == change.UserId))
            {
                post.UserPhotoUrl = change.UserPhotoUrl;
            }
            foreach (var post in MyPosts.Where(post => post.UserId == change.UserId))
            {
                post.UserPhotoUrl = change.UserPhotoUrl;
            }
        }
    }

    public void Dispose()
    {
        _realTimeUpdatesService?.RemoveHandler(nameof(ProfileViewModel));
        if (FollowViewModel is IDisposable disposableFollowViewModel)
        {
            disposableFollowViewModel.Dispose();
        }
        Console.WriteLine("[DEBUG] ProfileViewModel disposed");
    }

    #endregion

    #region Methods

    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        await Shell.Current.GoToAsync("//HomePage");
        Console.WriteLine("[DEBUG] Navigating to SettingsPage");
    }

    [RelayCommand]
    public async Task RefreshAllDataAsync()
    {
        _myPostStartIndex = 0;
        _bookMarkedPostStartIndex = 0;
        _likedPostStartIndex = 0;

        MyPosts.Clear();
        BookMarkedPost.Clear();
        LikedPosts.Clear();

        await FetchPostsAsync();
        Console.WriteLine("[DEBUG] Refreshed all data");
    }

    #endregion
}