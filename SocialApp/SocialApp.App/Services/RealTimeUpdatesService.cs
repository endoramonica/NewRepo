using Microsoft.AspNetCore.SignalR.Client;
using SocialAppLibrary.Shared;
using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.IHub;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialApp.App.Services;

public class RealTimeUpdatesService
{
    private readonly Dictionary<string, Action<PostDto>> _postChangeActions = new Dictionary<string, Action<PostDto>>();
    private readonly Dictionary<string, Action<Guid>> _postDeleteActions = new Dictionary<string, Action<Guid>>();
    private readonly Dictionary<string, Action<Guid, Guid>> _postLikeActions = new Dictionary<string, Action<Guid, Guid>>();
    private readonly Dictionary<string, Action<Guid, Guid>> _postUnLikeActions = new Dictionary<string, Action<Guid, Guid>>();
    private readonly Dictionary<string, Action<Guid, Guid>> _postBookmarkActions = new Dictionary<string, Action<Guid, Guid>>();
    private readonly Dictionary<string, Action<Guid, Guid>> _postUnBookmarkActions = new Dictionary<string, Action<Guid, Guid>>();
    private readonly Dictionary<string, Action<CommentDto>> _commentAddedActions = new Dictionary<string, Action<CommentDto>>();
    private readonly Dictionary<string, Action<UserPhotoChange>> _userPhotoChangeActions = new Dictionary<string, Action<UserPhotoChange>>();
    private readonly Dictionary<string, Action<NotificationDto>> _notificationActions = new Dictionary<string, Action<NotificationDto>>();
    private readonly Dictionary<string, Action<FollowNotificationDto>> _followNotificationActions = new Dictionary<string, Action<FollowNotificationDto>>();
    private HubConnection? _hubConnection;

    public RealTimeUpdatesService()
    {
        ConfigureRealTimeUpdates();
    }

    public void AddPostChangeAction(string key, Action<PostDto> action)
        => _postChangeActions[key] = action;

    public void AddPostDeleteAction(string key, Action<Guid> action)
        => _postDeleteActions[key] = action;

    public void AddPostLikeAction(string key, Action<Guid, Guid> action)
        => _postLikeActions[key] = action;

    public void AddPostUnLikeAction(string key, Action<Guid, Guid> action)
        => _postUnLikeActions[key] = action;

    public void AddPostBookmarkAction(string key, Action<Guid, Guid> action)
        => _postBookmarkActions[key] = action;

    public void AddPostUnBookmarkAction(string key, Action<Guid, Guid> action)
        => _postUnBookmarkActions[key] = action;

    public void AddCommentAddedAction(string key, Action<CommentDto> action)
        => _commentAddedActions[key] = action;

    public void AddUserPhotoChangeAction(string key, Action<UserPhotoChange> action)
        => _userPhotoChangeActions[key] = action;

    public void AddNotificationGeneratedAction(string key, Action<NotificationDto> action)
        => _notificationActions[key] = action;
    public void AddFollowNotificationAction(string key, Action<FollowNotificationDto> action) // Thêm mới
            => _followNotificationActions[key] = action;
    private void InvokeActions<T>(Dictionary<string, Action<T>> actions, T arg)
    {
        if (actions == null || arg == null)
            return;

        foreach (var (key, action) in actions)
        {
            try
            {
                action?.Invoke(arg);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in action {key}: {ex.Message}");
            }
        }
    }

    private void InvokeActions<T1, T2>(Dictionary<string, Action<T1, T2>> actions, T1 arg1, T2 arg2)
    {
        if (actions == null)
            return;

        foreach (var (key, action) in actions)
        {
            try
            {
                action?.Invoke(arg1, arg2);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in action {key}: {ex.Message}");
            }
        }
    }

    private async Task ConfigureRealTimeUpdates()
    {
        try
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(AppConstants.HubFullUrl)
                .Build();

            _hubConnection.On<PostDto>(nameof(ISocialHubClient.PostChange), postDto =>
            {
                InvokeActions(_postChangeActions, postDto);
            });

            _hubConnection.On<Guid>(nameof(ISocialHubClient.PostDelete), postId =>
            {
                InvokeActions(_postDeleteActions, postId);
            });

            _hubConnection.On<Guid, Guid>(nameof(ISocialHubClient.PostLike), (postId, userId) =>
            {
                InvokeActions(_postLikeActions, postId, userId);
            });

            _hubConnection.On<Guid, Guid>(nameof(ISocialHubClient.PostUnLike), (postId, userId) =>
            {
                InvokeActions(_postUnLikeActions, postId, userId);
            });

            _hubConnection.On<Guid, Guid>(nameof(ISocialHubClient.PostBookmark), (postId, userId) =>
            {
                InvokeActions(_postBookmarkActions, postId, userId);
            });

            _hubConnection.On<Guid, Guid>(nameof(ISocialHubClient.PostUnBookmark), (postId, userId) =>
            {
                InvokeActions(_postUnBookmarkActions, postId, userId);
            });

            _hubConnection.On<CommentDto>(nameof(ISocialHubClient.PostCommentAdded), commentDto =>
            {
                InvokeActions(_commentAddedActions, commentDto);
            });

            _hubConnection.On<UserPhotoChange>(nameof(ISocialHubClient.UserPhotoChange), userPhotoChange =>
            {
                InvokeActions(_userPhotoChangeActions, userPhotoChange);
            });

            _hubConnection.On<NotificationDto>(nameof(ISocialHubClient.NotificationGenerated), notification =>
            {
                InvokeActions(_notificationActions, notification);
            });
            _hubConnection.On<FollowNotificationDto>(nameof(ISocialHubClient.FollowNotification), notification => // Thêm mới
            {
                InvokeActions(_followNotificationActions, notification);
            });
            await _hubConnection.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting hub connection: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", $"Error setting up real-time updates: {ex.Message}", "OK");
        }
    }

    public void RemoveHandler(string key)
    {
        if (string.IsNullOrEmpty(key))
            return;

        _postChangeActions.Remove(key);
        _postDeleteActions.Remove(key);
        _postLikeActions.Remove(key);
        _postUnLikeActions.Remove(key);
        _postBookmarkActions.Remove(key);
        _postUnBookmarkActions.Remove(key);
        _commentAddedActions.Remove(key);
        _userPhotoChangeActions.Remove(key);
        _notificationActions.Remove(key);
        _followNotificationActions.Remove(key);
    }
}

