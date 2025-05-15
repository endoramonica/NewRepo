using SocialApp.App.Models;
using SocialApp.App.ViewModels;

namespace SocialApp.App.Controls;

public partial class PostActionButtons : ContentView
{
    public PostActionButtons()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty PostProperty =
        BindableProperty.Create(nameof(Post), typeof(PostModel), typeof(PostActionButtons), defaultBindingMode: BindingMode.OneWay);

    public PostModel Post
    {
        get => (PostModel)GetValue(PostProperty);
        set => SetValue(PostProperty, value);
    }

    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(PostBaseViewModel), typeof(PostActionButtons), defaultBindingMode: BindingMode.OneWay);

    public PostBaseViewModel ViewModel
    {
        get => (PostBaseViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
}
