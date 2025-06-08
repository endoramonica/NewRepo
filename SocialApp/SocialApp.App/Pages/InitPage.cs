using SocialApp.App.Services;

namespace SocialApp.App.Pages;

public class InitPage : ContentPage
{
    private readonly AuthService _authService;
    public const string FirstRunKey = "IsFirstRun";

    public InitPage(AuthService authService)
    {
        _authService = authService;

        Content = new VerticalStackLayout
        {
            Padding = 20,
            Spacing = 10,
            Children =
            {
                new Label
                {
                    Text = "Chào mừng bạn đến với ứng dụng!",
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = 20
                }
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _authService.Initialize(); // Khởi tạo AuthService
        if (!Preferences.Default.ContainsKey(FirstRunKey))
        {
            Preferences.Set(FirstRunKey, true);
            await Shell.Current.GoToAsync($"//{nameof(OnboardingPage)}");
            return;
        }

        

        if (_authService.IsLoggedIn)
        {
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            return;
        }

        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}
