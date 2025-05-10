using Microsoft.Maui.Controls;

namespace SocialApp.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent(); // Khởi tạo các thành phần UI từ file XAML
        }
        // Sự kiện khi nhấn vào "Remember Me"
        private void OnRememberMeTapped(object sender, TappedEventArgs e)
        {
            // Đảo trạng thái của CheckBox khi nhấn vào Label
            RememberMeCheckBox.IsChecked = !RememberMeCheckBox.IsChecked;
        }
        // Xử lý sự kiện khi người dùng nhấn vào "Forgot Password?"
        private async void OnForgotPasswordTapped(object sender, TappedEventArgs e)
        {
            await DisplayAlert("Forgot Password", "Redirecting to password reset page...", "OK");
        }
        private async void OnSignUpTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage()); // Điều hướng đến trang đăng ký
        }

        // Xử lý sự kiện khi người dùng nhấn vào biểu tượng Google
        private async void OnGoogleSignInTapped(object sender, TappedEventArgs e)
        {
            await DisplayAlert("Google Sign-In", "Google sign-in logic goes here", "OK");
        }

        // Xử lý sự kiện khi người dùng nhấn vào biểu tượng Facebook
        private async void OnFacebookSignInTapped(object sender, TappedEventArgs e)
        {
            await DisplayAlert("Facebook Sign-In", "Facebook sign-in logic goes here", "OK");
        }

        // Xử lý sự kiện khi người dùng nhấn vào biểu tượng Apple
        private async void OnAppleSignInTapped(object sender, TappedEventArgs e)
        {
            await DisplayAlert("Apple Sign-In", "Apple sign-in logic goes here", "OK");
        }
    }
}