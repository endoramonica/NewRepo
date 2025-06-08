using SocialApp.App.ViewModels;

namespace SocialApp.App.Pages;

public partial class SignUpPage : ContentPage
{
	public SignUpPage(RegisterViewModel registerViewModel)
	{
		InitializeComponent();
		BindingContext = registerViewModel;
	}

    

    
}