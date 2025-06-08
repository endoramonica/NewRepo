using System;
using Microsoft.Maui.Controls;
using SocialApp.App.Services;
using SocialApp.App.ViewModels;

namespace SocialApp.App.Pages
{
    public partial class HomePage : ContentPage
    {
        private readonly HomeViewModels _homeViewModels;
        private readonly RealTimeUpdatesService _realTimeUpdatesService;

        public HomePage( HomeViewModels homeViewModels , RealTimeUpdatesService realTimeUpdatesService)
        {
            InitializeComponent();
            BindingContext = homeViewModels;
            _homeViewModels = homeViewModels;
            _realTimeUpdatesService = realTimeUpdatesService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _homeViewModels.ConfigureRealTimeUpdates();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _realTimeUpdatesService.RemoveHandler(nameof(HomeViewModels));
        }

    }
}