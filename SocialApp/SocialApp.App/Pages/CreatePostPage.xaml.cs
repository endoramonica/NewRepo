using Microsoft.Maui.Controls;
using SocialApp.App.ViewModels;

namespace SocialApp.App.Pages
{
    public partial class CreatePostPage : ContentPage
    {
        public CreatePostPage(SavePostViewModel savePostViewModel)
        {
            InitializeComponent();
            BindingContext = savePostViewModel;
        }

        private void OnAvatarTapped(object sender, EventArgs e)
        {
            DisplayAlert("Thông báo", "Bạn đã nhấn vào avatar", "OK");
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
        

        

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//HomePage");
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }
    }
}