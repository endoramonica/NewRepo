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

namespace SocialApp.App.ViewModels;

[QueryProperty(nameof(CropPhotoSource), "new-src")]
public partial class ProfileViewModel : PostBaseViewModel, IDisposable
{
    private readonly IFollowApi _followApi;
    private readonly AuthService _authService;
    private readonly IUserApi _userApi;
    private readonly RealTimeUpdatesService _realTimeUpdatesService;

    public FollowViewModel FollowViewModel { get; }

    public ProfileViewModel(IPostApi postApi, IFollowApi followApi, AuthService authService, IUserApi userApi, RealTimeUpdatesService realTimeUpdatesService) : base(postApi)
    {
        _followApi = followApi;
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _userApi = userApi;
        _realTimeUpdatesService = realTimeUpdatesService;
        User = _authService.User ?? throw new InvalidOperationException("User must not be null.");
        FollowViewModel = new FollowViewModel(postApi, followApi, userApi, authService, realTimeUpdatesService);
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
    [NotifyPropertyChangedFor(nameof(DisplayPhotoUrl))]  // ✅ THÊM: Notify khi User thay đổi
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

    // ✅ SỬA: Sử dụng tên file ảnh mặc định đúng
    public string DisplayPhotoUrl => string.IsNullOrWhiteSpace(User?.PhotoUrl) ? "add_a_photo.png" : User.PhotoUrl;

    public UserModel CurrentUserModel => User != null ? UserModel.FromLoggedInUser(User) : new UserModel();

    // ❌ XÓA: Method này không cần thiết vì đã có NotifyPropertyChangedFor
    // async partial void OnUserChanged(LoggedInUser? oldValue, LoggedInUser newValue)
    // {
    //     OnPropertyChanged(nameof(DisplayPhotoUrl));
    //     OnPropertyChanged(nameof(CurrentUserModel));
    // }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        if (await App.Current.MainPage.DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No"))
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
        MyPosts.Clear(); // ✅ THÊM: Clear trước khi fetch
        await FetchMyPostsAsync();
    }

    [RelayCommand]
    private async Task SelectBookMarkedPostsTabAsync()
    {
        SelectedTab = ProfileTab.Bookmarked;
        _bookMarkedPostStartIndex = 0;
        BookMarkedPost.Clear(); // ✅ THÊM: Clear trước khi fetch
        await FetchBookmarkedPostsAsync();
    }

    [RelayCommand]
    private async Task SelectLikedPostsTabAsync()
    {
        SelectedTab = ProfileTab.Liked;
        _likedPostStartIndex = 0;
        LikedPosts.Clear(); // ✅ THÊM: Clear trước khi fetch
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
        var selectedPhotoSource = await ChoosePhotoAsync();
        if (!string.IsNullOrWhiteSpace(selectedPhotoSource))
        {
            var param = new Dictionary<string, object>
            {
                [nameof(CropImagePage.PhotoSource)] = selectedPhotoSource
            };
            await Shell.Current.GoToAsync("//CropImagePage", param);
        }
    }

    [ObservableProperty]
    private string? _cropPhotoSource;

    async partial void OnCropPhotoSourceChanged(string? oldValue, string? newValue)
    {
        if (!string.IsNullOrWhiteSpace(newValue))
        {
            await MakeApiCall(async () =>
            {
                var photoName = Path.GetFileName(newValue);
                using var fs = File.OpenRead(newValue);
                var photoStreamPart = new StreamPart(fs, photoName);

                var result = await _userApi.ChangePhotoAsync(photoStreamPart);
                if (!result.IsSuccess)
                {
                    await ShowErrorAlertAsync(result.Error);
                    return;
                }

                var newPhotoUrl = result.Data;

                // Cập nhật User - NotifyPropertyChangedFor sẽ tự động notify DisplayPhotoUrl và CurrentUserModel
                User = User with { PhotoUrl = newPhotoUrl };
                _authService.Login(new LoginResponse(User, _authService.Token));

                // Cập nhật ảnh trong tất cả posts của user hiện tại
                UpdateUserPhotoInPosts(newPhotoUrl);
            });
        }
    }

    private void UpdateUserPhotoInPosts(string newPhotoUrl)
    {
        var currentUserId = User.ID;

        // Cập nhật trong MyPosts
        foreach (var post in MyPosts.Where(p => p.UserId == currentUserId))
        {
            post.UserPhotoUrl = newPhotoUrl;
        }

        // Cập nhật trong BookMarkedPost nếu là post của user hiện tại
        foreach (var post in BookMarkedPost.Where(p => p.UserId == currentUserId))
        {
            post.UserPhotoUrl = newPhotoUrl;
        }

        // Cập nhật trong LikedPosts nếu là post của user hiện tại
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
    }

    private void UpdatePostInCollection(ObservableCollection<PostModel> collection, PostDto post)
    {
        var existing = collection.FirstOrDefault(p => p.PostId == post.PostId);
        if (existing != null)
        {
            existing.Content = post.Content;
            existing.IsBookmarked = post.IsBookmarked;
            existing.IsLiked = post.IsLiked;
            // ✅ THÊM: Cập nhật thêm các field khác nếu cần
            existing.UserPhotoUrl = post.UserPhotoUrl;
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
        if (myPost != null) MyPosts.Remove(myPost);

        var bookmarked = BookMarkedPost.FirstOrDefault(p => p.PostId == postId);
        if (bookmarked != null) BookMarkedPost.Remove(bookmarked);

        var liked = LikedPosts.FirstOrDefault(p => p.PostId == postId);
        if (liked != null) LikedPosts.Remove(liked);
    }

    private void OnUserPhotoChanged(UserPhotoChange change)
    {
        // Nếu là user hiện tại thay đổi ảnh
        if (change.UserId == User.ID)
        {
            // Cập nhật User object - NotifyPropertyChangedFor sẽ tự động notify UI
            User = User with { PhotoUrl = change.UserPhotoUrl };
            _authService.Login(new LoginResponse(User, _authService.Token));

            // Cập nhật trong posts
            UpdateUserPhotoInPosts(change.UserPhotoUrl);
        }
        else
        {
            // Cập nhật ảnh của user khác trong các posts
            foreach (var post in BookMarkedPost.Where(post => post.UserId == change.UserId))
            {
                post.UserPhotoUrl = change.UserPhotoUrl;
            }
            foreach (var post in LikedPosts.Where(post => post.UserId == change.UserId))
            {
                post.UserPhotoUrl = change.UserPhotoUrl;
            }
            // ✅ SỬA: Thêm cập nhật cho MyPosts nếu có post của user khác
            foreach (var post in MyPosts.Where(post => post.UserId == change.UserId))
            {
                post.UserPhotoUrl = change.UserPhotoUrl;
            }
        }
    }

    public void Dispose()
    {
        _realTimeUpdatesService?.RemoveHandler(nameof(ProfileViewModel));
        // ✅ THÊM: Dispose FollowViewModel nếu nó implement IDisposable
        if (FollowViewModel is IDisposable disposableFollowViewModel)
        {
            disposableFollowViewModel.Dispose();
        }
    }

    #endregion

    #region Methods

    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        await Shell.Current.GoToAsync("//SettingsPage");
    }

    // ✅ THÊM: Method để refresh tất cả data
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
    }

    #endregion
}