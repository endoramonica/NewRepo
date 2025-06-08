using SocialApp.App.Services;
using SocialApp.App.ViewModels;

namespace SocialApp.App.Pages;

public partial class FriendsPage : ContentPage
{
    private readonly FriendViewModel _friendViewModel;
    private readonly RealTimeUpdatesService _realTimeUpdatesService;

    public FriendsPage(FriendViewModel friendViewModel, RealTimeUpdatesService realTimeUpdatesService)
    {
        InitializeComponent();
        _friendViewModel = friendViewModel;
        _realTimeUpdatesService = realTimeUpdatesService;
        BindingContext = _friendViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        this.TranslationY = 800;
        this.Opacity = 0;

        await Task.WhenAll(
            this.TranslateTo(0, 0, 600, Easing.CubicOut),
            this.FadeTo(1, 600)
        );
        await _friendViewModel.LoadFriendsCommand.ExecuteAsync(null);
        await _friendViewModel.LoadPendingRequestsCommand.ExecuteAsync(null);

        _friendViewModel.ConfigureRealTimeUpdates();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _realTimeUpdatesService.RemoveHandler(nameof(FriendViewModel));
    }
}