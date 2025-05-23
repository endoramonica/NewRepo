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
using System.Threading.Tasks;

namespace SocialApp.App.ViewModels;

[QueryProperty(nameof(CropPhotoSource), "new-src")]
public partial class ProfileViewModel : PostBaseViewModel
{
    private readonly AuthService _authService;
    private readonly IUserApi _userApi;
    private readonly RealTimeUpdatesService _realTimeUpdatesService;

    public ProfileViewModel(IPostApi postApi, AuthService authService, IUserApi userApi , RealTimeUpdatesService realTimeUpdatesService) : base(postApi)
    {
        _authService = authService!;
        _userApi = userApi;
       _realTimeUpdatesService = realTimeUpdatesService;
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
    protected override void OnToggleBookmarkedAsync(PostModel postModel)
    {
        var currentPost = BookMarkedPost.FirstOrDefault(p => p.PostId == postModel.PostId);

        if (currentPost != null && !postModel.IsBookmarked)
        {
            BookMarkedPost.Remove(currentPost);

        }
    }
    protected override void OnToggleLikedAsync(PostModel postModel)
    {
        var currentPost = LikedPosts.FirstOrDefault(p => p.PostId == postModel.PostId);

        if (currentPost != null && !postModel.IsLiked)
        {
            LikedPosts.Remove(currentPost);

        }
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
    #region Real-time Events

    public void ConfigureRealTimeUpdates()
    {
        _realTimeUpdatesService.AddPostChangeAction(nameof(ProfileViewModel), OnPostChange);
        _realTimeUpdatesService.AddPostDeleteAction(nameof(ProfileViewModel), OnPostDeleted);
        _realTimeUpdatesService.AddUserPhotoChangeAction(nameof(ProfileViewModel), OnUserPhotoChanged);
        //_realTimeUpdatesService.AddNotificationGeneratedAction(nameof(ProfileViewModel), OnNotificationGenerated);
    }

    private void OnPostChange(PostDto post)
    {
        var myPost = MyPosts.FirstOrDefault(p => p.PostId == post.PostId);
        if (myPost != null)
        {
            myPost.Content = post.Content;
            // myPost.ImageUrl = post.ImageUrl;
            myPost.IsBookmarked = post.IsBookmarked;
            myPost.IsLiked = post.IsLiked;
            //myPost.LikeCount = post.LikeCount;
            //myPost.BookmarkCount = post.BookmarkCount;
        }
        var myBookmarkedPost = BookMarkedPost.FirstOrDefault(p => p.PostId == post.PostId);
        if (myBookmarkedPost != null) {
            myBookmarkedPost.Content = post.Content;
            // myPost.ImageUrl = post.ImageUrl;
            myBookmarkedPost.IsBookmarked = post.IsBookmarked;
            myBookmarkedPost.IsLiked = post.IsLiked;
            //myPost.LikeCount = post.LikeCount;
            //myPost.BookmarkCount = post.BookmarkCount;
        }
        var myLikedPost = LikedPosts.FirstOrDefault(p => p.PostId == post.PostId);
        if (myLikedPost != null)
        {
            myLikedPost.Content = post.Content;
            // myPost.ImageUrl = post.ImageUrl;
            myLikedPost.IsBookmarked = post.IsBookmarked;
            myLikedPost.IsLiked = post.IsLiked;
            //myPost.LikeCount = post.LikeCount;
            //myPost.BookmarkCount = post.BookmarkCount;
        }
    }

    private  void  OnPostDeleted(Guid postId)
    {
        var myPost = MyPosts.FirstOrDefault(p => p.PostId == postId);
        if (myPost != null)
        {
           MyPosts.Remove(myPost);
        }
        var myBookmarkedPost = BookMarkedPost.FirstOrDefault(p => p.PostId == postId);
        if (myBookmarkedPost != null)
        {
            BookMarkedPost.Remove(myBookmarkedPost);
        }
        var myLikedPost = LikedPosts.FirstOrDefault(p => p.PostId == postId);
        if (myLikedPost != null)
        {
            LikedPosts.Remove(myLikedPost);
        }
    }

    private void OnUserPhotoChanged(UserPhotoChange change)
    {
        if (change.UserId == User.ID)
        {
            foreach (var post in MyPosts)
            { post.UserPhotoUrl = change.UserPhotoUrl; }
            foreach (var post in BookMarkedPost.Where(post=>post.UserId == change.UserId))
            { post.UserPhotoUrl = change.UserPhotoUrl; }
            foreach (var post in LikedPosts.Where(post => post.UserId == change.UserId))
            { post.UserPhotoUrl = change.UserPhotoUrl; }
        }
    }

    

    #endregion
    #region Methods
    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        
        await Shell.Current.GoToAsync("//HomePage");
    }
    
    #endregion
}
