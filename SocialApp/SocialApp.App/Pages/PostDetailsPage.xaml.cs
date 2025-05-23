
using Microsoft.Maui.Controls;
using SocialApp.App.Services;
using SocialApp.App.ViewModels;

namespace SocialApp.App.Pages
{
    public partial class PostDetailsPage : ContentPage
    {
        private readonly DetailsViewModel _detailsViewModel;
        private readonly RealTimeUpdatesService _realTimeUpdatesService;

        public PostDetailsPage( DetailsViewModel detailsViewModel , RealTimeUpdatesService realTimeUpdatesService)
        {
            InitializeComponent();
            BindingContext = detailsViewModel;
            _detailsViewModel = detailsViewModel;
            _realTimeUpdatesService = realTimeUpdatesService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _detailsViewModel.ConfigureRealTimeUpdates();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _realTimeUpdatesService.RemoveHandler(nameof(DetailsViewModel));
        }



        private void Button_Clicked(object sender, EventArgs e)
        {

        }
    }
}
