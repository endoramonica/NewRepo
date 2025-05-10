namespace SocialApp.Pages;

public partial class LandingPage : ContentPage
{
	public LandingPage()
	{
		InitializeComponent();
	}

    private async void OnGetStartedClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///LoginPage"); // Chuyển đến trang LoginPage
    }
}