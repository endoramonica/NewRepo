
namespace SocialApp.App.Pages;

public partial class OnboardingPage : ContentPage
{
	public OnboardingPage()
	{
		InitializeComponent();
	}

    private async void OnGetStartedClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///LoginPage"); // Chuyển đến trang LoginPage
        //Preferences.Default.Set(InitPage.FirstRunKey, true);
    }





}