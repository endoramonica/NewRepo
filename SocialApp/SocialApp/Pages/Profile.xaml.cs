namespace SocialApp.Pages;

public partial class Profile : ContentPage
{
	public Profile()
	{
		InitializeComponent();
	}

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {

    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {

    }

    private void ToolbarItem_Clicked_2(object sender, EventArgs e)
    {

    }

    private void ToolbarItem_Clicked_3(object sender, EventArgs e)
    {

    }

    private async void ToolbarItem_Clicked_4(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
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
    }

}