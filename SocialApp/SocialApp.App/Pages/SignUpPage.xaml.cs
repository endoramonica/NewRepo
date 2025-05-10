using SocialApp.App.ViewModels;

namespace SocialApp.App.Pages;

public partial class SignUpPage : ContentPage
{
	public SignUpPage(RegisterViewModel registerViewModel)
	{
		InitializeComponent();
		BindingContext = registerViewModel;
	}

    

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///LoginPage");
    }
}