using CommunityToolkit.Mvvm.Input;
using SocialApp.App.Apis;
using SocialApp.App.Models;
using SocialAppLibrary.Shared.Dtos;
using System.Diagnostics;

namespace SocialApp.App.ViewModels;

public partial class PostBaseViewModel:BaseViewModel 
{
    public  IPostApi PostApi { get;  }

   

    public PostBaseViewModel(IPostApi postApi) // inject thêm
    {
        PostApi = postApi;
        
    }

    protected bool SkipGoToDetailsPage { get; set; }
    protected virtual void OnToggleBookmarkedAsync(PostModel postModel)
    {
        // This method can be overridden in derived classes to handle the toggle bookmark action
        // For example, you can update the UI or perform additional actions here
    }
    protected virtual void OnToggleLikedAsync(PostModel postModel)
    {
        // This method can be overridden in derived classes to handle the toggle bookmark action
        // For example, you can update the UI or perform additional actions here
    }

    [RelayCommand]
    private async Task GoToPostDetailsPageAsync(PostModel post)
    {
        if (SkipGoToDetailsPage)
        {

            return;
        }
        var param = new Dictionary<string, object>
        {
            [nameof(DetailsViewModel.Post)] = post
        };
        await NavigationAsync("///PostDetailsPage", param);
    }
    [RelayCommand]
    private async Task GoToHomePageAsync()
    {
        await NavigationAsync("///HomePage");
    }

    [RelayCommand]
    private async Task ToggleLikeAsync(PostModel post)
    {
        await MakeApiCall(async () =>
        {
            var originalState = post.IsLiked;
            var result = await PostApi.ToggleLikeAsync(post.PostId);
            post.IsLiked = !post.IsLiked;
            if (!result.IsSuccess)
            {
               await ShowErrorAlertAsync(result.Error);
                post.IsLiked = originalState;
                return;
            }
            OnToggleLikedAsync(post);
        });
    }
    [RelayCommand]
    private async Task ToggleBookMarkAsync(PostModel post)
    {
        await MakeApiCall(async () =>   
        {
            var originalState = post.IsBookmarked;
            var result = await PostApi.ToggleBookmarkAsync(post.PostId); // <-- FIXED
            post.IsBookmarked = !post.IsBookmarked;
            if (!result.IsSuccess)
            {
                await ShowErrorAlertAsync(result.Error);
                post.IsBookmarked = originalState;
                return;
            }
            OnToggleBookmarkedAsync(post);

        });
    }

    [RelayCommand]
    private async Task SharePostAsync(PostModel post)
    {
        string? tempPhotoPath = null;

        if (!string.IsNullOrWhiteSpace(post.PhotoUrl))
        {
            tempPhotoPath = await DownloadPhotosAsync(post.PhotoUrl);
            if (string.IsNullOrWhiteSpace(tempPhotoPath))
            {
                await ShowErrorAlertAsync("Không thể tải ảnh để chia sẻ.");
                return;
            }
        }

        try
        {
            if (!string.IsNullOrWhiteSpace(tempPhotoPath))
            {
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = $"Bài viết từ {post.UserName ?? "ai đó"}",
                    File = new ShareFile(tempPhotoPath)
                });
            }
            else
            {
                // Không có ảnh, chia sẻ văn bản
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Title = "Maui Social",
                    Text = $"Check out this post by {post.UserName ?? "someone"}!"
                });
            }
        }
        catch (Exception)
        {
            await ShowErrorAlertAsync("Đã xảy ra lỗi khi chia sẻ bài viết.");
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(tempPhotoPath) && File.Exists(tempPhotoPath))
            {
                File.Delete(tempPhotoPath);
            }
        }
    }

    private Dictionary<string, string> _downloadPhotos = [];
    private HttpClient? _httpClient;
    private async Task<string?> DownloadPhotosAsync(string photoUrl)
    {
        if (_downloadPhotos.TryGetValue(photoUrl, out var localPhotoPath))
        {   // has been downloaded
            return localPhotoPath;
        }
        IsBusy = true;
        try {
            _httpClient ??= new HttpClient();
            var photoBytes = await _httpClient.GetByteArrayAsync(photoUrl);

            var localPath = Path.Combine(FileSystem.CacheDirectory,"share");
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }
            var photoName = Path.GetFileName(photoUrl);
             localPhotoPath = Path.Combine(localPath, photoName);
            _downloadPhotos[photoUrl] = localPhotoPath;
            File.WriteAllBytes(localPhotoPath, photoBytes);
            return localPhotoPath;
        }
        catch (Exception ex)
        {
            await ShowErrorAlertAsync(ex.Message);
            return null;
        }
        finally
        {
            IsBusy = false;
        }

    }
    
    public void Dipose()
    {
        _httpClient?.Dispose();
        // clean up the download local.caches load photos 
        foreach (var (_, localPhotoPath) in _downloadPhotos)
        {

            if (File.Exists(localPhotoPath))
            {
                File.Delete(localPhotoPath);
            }
        }
    }
   
}




