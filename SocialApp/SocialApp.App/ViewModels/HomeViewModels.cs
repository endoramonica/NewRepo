    using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refit;
using SocialApp.App.Apis;
using SocialApp.App.Models;
using SocialApp.App.Pages;
using SocialAppLibrary.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApp.App.ViewModels
{
    public partial class HomeViewModels:PostBaseViewModel
    {
       
        public HomeViewModels(IPostApi postApi) : base(postApi) 
        {
          
            FetchPostAsync();
        }
        public ObservableCollection<PostModel> Posts { get; set; } = [];
        //public ObservableCollection<PostDto> Posts { get; set; } = new ObservableCollection<PostDto>();

        private int _startIndex = 0; 
        private const int PageSize = 10;
        

        [RelayCommand]
        private async Task FetchPostAsync ()
        {
            await MakeApiCall(async () =>
            {
                var posts = await PostApi.GetPostsAsync(_startIndex, PageSize);
                if (posts.Length > 0)
                {
                    if (_startIndex == 0 && Posts.Count > 0)
                    {
                        //this is pull to refresh case 
                        //resey the post observable collection 
                        Posts.Clear();

                    }
                    _startIndex += posts.Length;
                    foreach (var p in posts)
                    {
                        Posts.Add(PostModel.FromDto(p));
                    }

                }
               


            });
            

        }
        [ObservableProperty]
        private bool _isRefreshView;
        [RelayCommand]
        private async Task RefreshPostAsync()
        {
            try
            {
                
                _startIndex = 0; // Reset the start index to fetch from the beginning
                await FetchPostAsync(); // Reuse the existing FetchPostAsync method
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🚨 [RefreshPostAsync] Error during refresh: {ex.Message}");
                await ShowErrorAlertAsync("Unable to refresh posts.");
            }
            finally
            {
                IsRefreshView = false; // Reset the refresh state
            }
        }

        
        #region Methods

        /// <summary>
        /// Điều hướng đến trang thông báo.
        /// </summary>
        /// <returns>Task hoàn thành khi điều hướng xong.</returns>
        [RelayCommand]
        private async Task NavigateToNotificationAsync()
        {
            try
            {
                await NavigationAsync("//NotificationPage");
                
            }
            catch (Exception ex)
            {
                
                await ShowErrorAlertAsync("Không thể điều hướng đến trang thông báo.");
            }
        }

        /// <summary>
        /// Điều hướng đến trang tạo bài viết.
        /// </summary>
        /// <returns>Task hoàn thành khi điều hướng xong.</returns>
        [RelayCommand]
        private async Task NavigateToCreatePostAsync()
        {
            try
            {
                await NavigationAsync("///CreatePostPage");
                Debug.WriteLine("➡️ [NavigateToCreatePostAsync] Điều hướng đến CreatePostPage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🚨 [NavigateToCreatePostAsync] Lỗi khi điều hướng: {ex.Message}");
                await ShowErrorAlertAsync("Không thể điều hướng đến trang tạo bài viết.");
            }
        }

        /// <summary>
        /// Điều hướng đến trang cá nhân và hiển thị thông báo.
        /// </summary>
        /// <returns>Task hoàn thành khi điều hướng xong.</returns>
        [RelayCommand]
        private async Task NavigateToProfileAsync()
        {
            try
            {
                await NavigationAsync("//Profile");
                Debug.WriteLine("➡️ [NavigateToProfileAsync] Điều hướng đến Profile");
                await ToastAsync("Chuyển đến trang cá nhân!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🚨 [NavigateToProfileAsync] Lỗi khi điều hướng: {ex.Message}");
                await ShowErrorAlertAsync("Không thể điều hướng đến trang cá nhân.");
            }
        }

        /// <summary>
        /// Điều hướng đến trang chi tiết bài viết.
        /// </summary>
        /// <returns>Task hoàn thành khi điều hướng xong.</returns>
        [RelayCommand]
        private async Task NavigateToPostDetailAsync()
        {
            try
            {
                await NavigationAsync("///PostDetailsPage");
                Debug.WriteLine("➡️ [NavigateToPostDetailAsync] Điều hướng đến PostDetailsPage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"🚨 [NavigateToPostDetailAsync] Lỗi khi điều hướng: {ex.Message}");
                await ShowErrorAlertAsync("Không thể điều hướng đến trang chi tiết bài viết.");
            }
        }

        #endregion
    }
}
