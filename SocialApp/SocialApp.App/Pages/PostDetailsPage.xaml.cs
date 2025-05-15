
using Microsoft.Maui.Controls;
using SocialApp.App.ViewModels;

namespace SocialApp.App.Pages
{
    public partial class PostDetailsPage : ContentPage
    {
        public PostDetailsPage( DetailsViewModel detailsViewModel)
        {
            InitializeComponent();
            BindingContext = detailsViewModel;
        }

        

        

        private void Button_Clicked(object sender, EventArgs e)
        {

        }
    }
}
