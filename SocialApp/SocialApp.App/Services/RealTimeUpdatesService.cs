using Microsoft.AspNetCore.SignalR.Client;
using SocialAppLibrary.Shared;
using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.IHub;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialApp.App.Services
{
    public class RealTimeUpdatesService
    {
        private readonly Dictionary<string, Action<PostDto>> _postChangeActions = new();
        private readonly Dictionary<string, Action<Guid>> _postDeleteActions = new();
        private readonly Dictionary<string, Action<Guid, Guid>> _postLikeActions = new();
        private readonly Dictionary<string, Action<Guid, Guid>> _postUnLikeActions = new();
        private readonly Dictionary<string, Action<Guid, Guid>> _postBookmarkActions = new();
        private readonly Dictionary<string, Action<Guid, Guid>> _postUnBookmarkActions = new();
        private readonly Dictionary<string, Action<CommentDto>> _commentAddedActions = new();
        private readonly Dictionary<string, Action<UserPhotoChange>> _userPhotoChangeActions = new();
        private readonly Dictionary<string, Action<NotificationDto>> _notificationActions = new();
        private readonly Dictionary<string, Action<FollowNotificationDto>> _followNotificationActions = new();
        private readonly Dictionary<string, Action<MessageDto>> _receivedMessageActions = new();
        // Thêm các dictionary cho chức năng kết bạn
        private readonly Dictionary<string, Action<FriendRequestNotificationDto>> _friendRequestSentActions = new();
        private readonly Dictionary<string, Action<FriendRequestNotificationDto>> _friendRequestAcceptedActions = new();
        private readonly Dictionary<string, Action<FriendRequestNotificationDto>> _friendRequestRejectedActions = new();
        private readonly Dictionary<string, Action<FriendRequestNotificationDto>> _friendRemovedActions = new();

        private HubConnection? _hubConnection;
        private bool _isInitialized;

        public RealTimeUpdatesService()
        {
            // Defer initialization to explicit call
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;
            await ConfigureRealTimeUpdates();
            _isInitialized = true;
        }

        public void AddPostChangeAction(string key, Action<PostDto> action)
            => _postChangeActions[key] = action;

        public void AddReceivedMessageHandler(string key, Action<MessageDto> action)
            => _receivedMessageActions[key] = action;

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

        public void AddFollowNotificationAction(string key, Action<FollowNotificationDto> action)
            => _followNotificationActions[key] = action;

        // Thêm các handler cho chức năng kết bạn
        public void AddFriendRequestSentAction(string key, Action<FriendRequestNotificationDto> action)
            => _friendRequestSentActions[key] = action;

        public void AddFriendRequestAcceptedAction(string key, Action<FriendRequestNotificationDto> action)
            => _friendRequestAcceptedActions[key] = action;

        public void AddFriendRequestRejectedAction(string key, Action<FriendRequestNotificationDto> action)
            => _friendRequestRejectedActions[key] = action;

        public void AddFriendRemovedAction(string key, Action<FriendRequestNotificationDto> action)
            => _friendRemovedActions[key] = action;

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
                    .WithAutomaticReconnect()
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

                _hubConnection.On<NotificationDto>(nameof(ISocialHubClient.ReceiveNotification), notification =>
                {
                    InvokeActions(_notificationActions, notification);
                });

                _hubConnection.On<FollowNotificationDto>(nameof(ISocialHubClient.FollowNotification), notification =>
                {
                    InvokeActions(_followNotificationActions, notification);
                });

                _hubConnection.On<MessageDto>(nameof(ISocialHubClient.ReceiveMessage), messageDto =>
                {
                    InvokeActions(_receivedMessageActions, messageDto);
                });

                // Đăng ký các sự kiện kết bạn
                _hubConnection.On<FriendRequestNotificationDto>(nameof(ISocialHubClient.FriendRequestSent), notification =>
                {
                    InvokeActions(_friendRequestSentActions, notification);
                });

                _hubConnection.On<FriendRequestNotificationDto>(nameof(ISocialHubClient.FriendRequestAccepted), notification =>
                {
                    InvokeActions(_friendRequestAcceptedActions, notification);
                });

                _hubConnection.On<FriendRequestNotificationDto>(nameof(ISocialHubClient.FriendRequestRejected), notification =>
                {
                    InvokeActions(_friendRequestRejectedActions, notification);
                });

                _hubConnection.On<FriendRequestNotificationDto>(nameof(ISocialHubClient.FriendRemoved), notification =>
                {
                    InvokeActions(_friendRemovedActions, notification);
                });

                _hubConnection.Closed += async (error) =>
                {
                    Console.WriteLine($"Hub connection closed: {error?.Message}");
                    await Task.Delay(5000); // Wait before reconnecting
                    await StartConnectionAsync();
                };

                await StartConnectionAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting hub connection: {ex.Message}");
                throw; // Re-throw for caller to handle
            }
        }

        private async Task StartConnectionAsync()
        {
            if (_hubConnection?.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await _hubConnection.StartAsync();
                    Console.WriteLine("Hub connection started successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error starting hub connection: {ex.Message}");
                    throw;
                }
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
            _receivedMessageActions.Remove(key);
            // Xóa các handler kết bạn
            _friendRequestSentActions.Remove(key);
            _friendRequestAcceptedActions.Remove(key);
            _friendRequestRejectedActions.Remove(key);
            _friendRemovedActions.Remove(key);
        }

        public async Task DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
                Console.WriteLine("Hub connection disposed");
            }
        }
    }
}