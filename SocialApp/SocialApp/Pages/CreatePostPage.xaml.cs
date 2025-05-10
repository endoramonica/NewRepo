using Microsoft.Maui.Controls;

namespace SocialApp.Pages
{
    public partial class CreatePostPage : ContentPage
    {
        public CreatePostPage()
        {
            InitializeComponent();
        }

        private void OnAvatarTapped(object sender, EventArgs e)
        {
            DisplayAlert("Thông báo", "Bạn đã nhấn vào avatar", "OK");
        }

        private void OnAddImageClicked(object sender, EventArgs e)
        {
            DisplayAlert("Thông báo", "Thêm hình ảnh/video", "OK");
        }

        private void OnAddEmotionClicked(object sender, EventArgs e)
        {
            DisplayAlert("Thông báo", "Thêm cảm xúc", "OK");
        }

        private void OnAddLocationClicked(object sender, EventArgs e)
        {
            DisplayAlert("Thông báo", "Thêm vị trí", "OK");
        }

        private void OnTagFriendsClicked(object sender, EventArgs e)
        {
            DisplayAlert("Thông báo", "Tag bạn bè", "OK");
        }

        private void OnPostClicked(object sender, EventArgs e)
        {
            DisplayAlert("Thông báo", "Bài viết đã được đăng", "OK");
        }

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//HomePage");
        }
    }
}