namespace SocialApp.App.Pages;

public partial class LandingPage : ContentPage
{
	public LandingPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync("//HomePage");
    }
}