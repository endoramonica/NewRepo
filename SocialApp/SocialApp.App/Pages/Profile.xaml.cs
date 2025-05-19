
using SocialApp.App.ViewModels;

namespace SocialApp.App.Pages;

public partial class Profile : ContentPage
{
    private readonly ProfileViewModel _viewModel;
    public Profile( ProfileViewModel profileViewModel)
	{
		InitializeComponent();
        _viewModel = profileViewModel;
        BindingContext = _viewModel;
    }
  
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.FetchMyPostsCommand.ExecuteAsync(null); // Gọi trực tiếp phương thức
        this.TranslationY = 800; // Đặt trang bên ngoài màn hình
        this.Opacity = 0;

        await Task.WhenAll(
            this.TranslateTo(0, 0, 600, Easing.CubicOut), // Trượt lên
            this.FadeTo(1, 600) // Làm mờ dần
        );
    }

}