using CommunityToolkit.Mvvm.Input;
using SocialApp.App.Services;
using SocialApp.App.ViewModels;

namespace SocialApp.App.Pages;

public partial class NotificationPage : ContentPage
{
    private readonly NotificationViewModel _notification;
    private readonly RealTimeUpdatesService _realTimeUpdatesService;

    public NotificationPage(NotificationViewModel notification, RealTimeUpdatesService realTimeUpdatesService)
    {
        InitializeComponent();
        BindingContext = notification;
        _notification = notification;
        _realTimeUpdatesService = realTimeUpdatesService;
    }
    private  async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage", animate: true);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        this.TranslationY = 800; // Đặt trang bên ngoài màn hình
        this.Opacity = 0;

        await Task.WhenAll(
            this.TranslateTo(0, 0, 600, Easing.CubicOut), // Trượt lên
            this.FadeTo(1, 600) // Làm mờ dần
        );
        _notification.ConfigureRealTimeUpdates();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _realTimeUpdatesService.RemoveHandler(nameof(NotificationViewModel));
    }
    public record NotificationModel(DateTime On , string Text)
	{
       
	}
    
}