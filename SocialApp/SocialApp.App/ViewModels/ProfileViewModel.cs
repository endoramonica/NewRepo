using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refit;
using SocialApp.App.Apis;
using SocialApp.App.Models;
using SocialApp.App.Pages;
using SocialApp.App.Services;
using SocialAppLibrary.Shared.Dtos;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SocialApp.App.ViewModels;

[QueryProperty(nameof(CropPhotoSource), "new-src")]
public partial class ProfileViewModel : PostBaseViewModel
{
    private readonly AuthService _authService;
    private readonly IUserApi _userApi;

    public ProfileViewModel(IPostApi postApi, AuthService authService, IUserApi userApi) : base(postApi)
    {
        _authService = authService!;
        _userApi = userApi;
        User = _authService.User!;
    }

    public enum ProfileTab
    {
        MyPosts,
        Bookmarked,
        Liked
    }

    // Người dùng đang đăng nhập
    [ObservableProperty]
    private LoggedInUser _user;

    // Tab hiện tại
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsMyPostTabSelected))]
    [NotifyPropertyChangedFor(nameof(IsBookMarkedTabSelected))]
    [NotifyPropertyChangedFor(nameof(IsLikedTabSelected))]
    private ProfileTab _selectedTab = ProfileTab.MyPosts;

    // Tab chọn kiểm tra logic
    public bool IsMyPostTabSelected => SelectedTab == ProfileTab.MyPosts;
    public bool IsBookMarkedTabSelected => SelectedTab == ProfileTab.Bookmarked;
    public bool IsLikedTabSelected => SelectedTab == ProfileTab.Liked;

    // Các danh sách bài viết
    public ObservableCollection<PostModel> MyPosts { get; set; } = [];
    public ObservableCollection<PostModel> BookMarkedPost { get; set; } = [];
    public ObservableCollection<PostModel> LikedPosts { get; set; } = [];

    // Chỉ số phân trang
    private int _myPostStartIndex = 0;
    private int _bookMarkedPostStartIndex = 0;
    private int _likedPostStartIndex = 0;

    private const int PageSize = 4;

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
        await FetchMyPostsAsync();
    }

    [RelayCommand]
    private async Task SelectBookMarkedPostsTabAsync()
    {
        SelectedTab = ProfileTab.Bookmarked;
        _bookMarkedPostStartIndex = 0;
        await FetchBookmarkedPostsAsync();
    }

    [RelayCommand]
    private async Task SelectLikedPostsTabAsync()
    {
        SelectedTab = ProfileTab.Liked;
        _likedPostStartIndex = 0;
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
                if (_myPostStartIndex == 0 && MyPosts.Count > 0)
                    MyPosts.Clear();

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
                if (_bookMarkedPostStartIndex == 0 && BookMarkedPost.Count > 0)
                    BookMarkedPost.Clear();

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
                if (_likedPostStartIndex == 0 && LikedPosts.Count > 0)
                    LikedPosts.Clear();

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

    [RelayCommand]
    private async Task ChangePhotoAsync()
    {
        var selectedPhotoSource = await ChoosePhotoAsync(); // Assuming ChoosetPhotoAsync is a typo
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
        if(!string.IsNullOrWhiteSpace(newValue))
        {
            await MakeApiCall(async () =>
            {
                var photoName = Path.GetFileName(newValue); // Assuming newValue is defined in the outer scope
                using var fs = File.OpenRead(newValue); // Assuming photoName here refers to the full path from newValue

                var photoStreamPart = new StreamPart(fs, photoName);

                var result = await _userApi.ChangePhotoAsync(photoStreamPart);
                if (!result.IsSuccess)
                {
                    await ShowErrorAlertAsync(result.Error);
                    return;
                }

                var newPhotoUrl = result.Data;

                User = User with { PhotoUrl = newPhotoUrl };
                _authService.Login(new LoginResponse(User, _authService.Token));

                // Change the user photo url in my posts
                foreach (var post in MyPosts)
                {
                    post.UserPhotoUrl = newPhotoUrl;
                }
            });
        }
    }
    #region Methods
    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        
        await Shell.Current.GoToAsync("//HomePage");
    }

    #endregion
}
