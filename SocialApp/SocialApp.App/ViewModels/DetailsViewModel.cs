using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SocialApp.App.Apis;
using SocialApp.App.Models;
using SocialApp.App.Pages;
using SocialApp.App.Services;
using SocialAppLibrary.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SocialApp.App.ViewModels;

[QueryProperty(nameof(Post), nameof(Post))] // Move the attribute to the class declaration
public partial class DetailsViewModel : PostBaseViewModel
{
    private readonly AuthService _authService;
    
   
    public DetailsViewModel(AuthService authService , IPostApi postApi) : base(postApi)
    { 
        _authService = authService;
       
        SkipGoToDetailsPage = true; // Skip the navigation to the details page
        //FetchCommentsAsync();
    }

    [ObservableProperty]
    private PostModel _post = new();

    [ObservableProperty]
    private bool _isOwner;
    //public ObservableCollection<CommentDto> Comments { get; set; } = new ObservableCollection<CommentDto>();
    public ObservableCollection<CommentDto> Comments { get; set; } = [];


     async partial void   OnPostChanged(PostModel value)
    {
        IsOwner = _authService.User?.ID == value.UserId; // Check if the current user is the owner of the post
        await FetchCommentsAsync();                                                    // Fetch the comments for the post
       
    }
    private int _startIndex = 0;
    private const int _pageSize = 10;
    [RelayCommand]
    private async Task FetchCommentsAsync()
    {
        await MakeApiCall(async () =>
        {
            var comments = await PostApi.GetCommentsAsync(Post.PostId, _startIndex, _pageSize);
            if (comments.Length > 0)
            {
                _startIndex += comments.Length; // Update the start index for pagination
                foreach (var c in comments)
                {
                    Comments.Add(c);
                }
            }
            else
            {
                await ToastAsync("No comments found.");
            }
        });
    }

    [ObservableProperty]
    private string? _commentContent;

    



    [RelayCommand]
    private async Task AddComment()
    {
        if (string.IsNullOrWhiteSpace(CommentContent))
        {
            await ToastAsync("Please enter a comment.");
            return;
        }
        await MakeApiCall(async () =>
        {
            if (Post.PostId != Guid.Empty) // Ensure PostId is not an empty Guid
            {
                var newCommentDto = new SaveCommentDto
                {
                    PostId = Post.PostId, // Explicitly cast nullable Guid to Guid
                    Content = CommentContent,
                };

                var result = await PostApi.SaveCommentAsync(Post.PostId, newCommentDto); // Explicitly cast nullable Guid to Guid
                if (!result.IsSuccess)
                {
                    await ShowErrorAlertAsync(result.Error);
                    return;
                }
                var addedComment = result.Data;
                Comments = [addedComment, .. Comments]; // Add the new comment to the collection
                                                        // Clear the comment entry field
                CommentContent = string.Empty; // Clear the input field
                OnPropertyChanged(nameof(Comments)); // Notify the UI about the change
                await ToastAsync("Comment added successfully.");
            }
            else
            {
                await ToastAsync("Invalid Post ID.");
            }
        });
    }

    [RelayCommand]
    private async Task DeletePost()
    {
        if (await Shell.Current.DisplayAlert("Delete Post", "Are you sure you want to delete this post?", "Yes", "No"))
        {
            await MakeApiCall(async () =>
            {
                if (Post.PostId != Guid.Empty) // Ensure PostId is not an empty Guid
                {
                    var result = await PostApi.DeletePostAsync(Post.PostId); // Explicitly cast nullable Guid to Guid
                    if (!result.IsSuccess)
                    {
                        await ShowErrorAlertAsync(result.Error);
                        return;
                    }
                }
                else
                {
                    await ToastAsync("Invalid Post ID.");
                }
            });
            // Call the API to delete the post

            await ToastAsync("Post deleted successfully.");
            await NavigationAsync("///HomePage");
        }
    }
    [RelayCommand]
    private async Task BackHomeAsync()
    {

        await NavigationAsync("//HomePage");
    }
    [RelayCommand]
    private async Task EditPostAsync(PostModel post)
    {
        var param = new Dictionary<string, object>()
        {
            [nameof(SavePostViewModel.Post)] = post
        };
        await NavigationAsync("///CreatePostPage",param);
    }
}

