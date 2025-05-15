using CommunityToolkit.Mvvm.Input;
using SocialApp.App.Apis;
using SocialApp.App.Models;
using SocialAppLibrary.Shared.Dtos;

namespace SocialApp.App.ViewModels;

public partial class PostBaseViewModel:BaseViewModel
{
    public  IPostApi PostApi { get;  }

    public PostBaseViewModel( IPostApi postApi)
    {
        PostApi = postApi;
    }
    protected bool SkipGoToDetailsPage { get; set; }

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
        });
    }
    [RelayCommand]
    private async Task ToggleBookMarkAsync(PostModel post)
    {
        await MakeApiCall(async () =>
        {
            var originalState = post.IsBookmarked;
            var result = await PostApi.ToggleLikeAsync(post.PostId);
            post.IsBookmarked = !post.IsBookmarked;
            if (!result.IsSuccess)
            {
                await ShowErrorAlertAsync(result.Error);
                post.IsBookmarked = originalState;
                return;
            }
        });
    }
}

