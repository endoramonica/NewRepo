using System;
using Microsoft.Maui.Controls;
using SocialApp.App.ViewModels;

namespace SocialApp.App.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage( HomeViewModels homeViewModels)
        {
            InitializeComponent();
            BindingContext = homeViewModels;
        }

    }
}