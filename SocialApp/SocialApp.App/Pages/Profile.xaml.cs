
using SocialApp.App.Services;
using SocialApp.App.ViewModels;

namespace SocialApp.App.Pages;

public partial class Profile : ContentPage
{
    private readonly ProfileViewModel _viewModel;
    private readonly RealTimeUpdatesService _realTimeUpdatesService;

    public Profile( ProfileViewModel profileViewModel , RealTimeUpdatesService realTimeUpdatesService )
	{
		InitializeComponent();
        _viewModel = profileViewModel;
        _realTimeUpdatesService = realTimeUpdatesService;
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
        _viewModel.ConfigureRealTimeUpdates();
    }
    
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _realTimeUpdatesService.RemoveHandler(nameof(ProfileViewModel));
    }

}