using System;
using Microsoft.Maui.Controls;

namespace SocialApp.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private async void OnNotificationTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//NotificationPage");
        }

        private async void OnChatTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CreatePostPage),animate: true);
        }

        private async void OnProfileTapped(object sender, EventArgs e)
        {
            // Điều hướng đến trang Profile nếu có
             await Shell.Current.GoToAsync("//Profile");
            //await DisplayAlert("Hồ sơ", "Chuyển đến trang cá nhân!", "OK");
        }

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("///PostDetailsPage");



        }

        private async void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("///CreatePostPage");
        }
    }
}