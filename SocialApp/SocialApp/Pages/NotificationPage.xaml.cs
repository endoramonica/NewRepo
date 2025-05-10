namespace SocialApp.Pages;

public partial class NotificationPage : ContentPage
{
	public NotificationPage()
	{
		InitializeComponent();
        List<NotificationModel> notifications = [
    new NotificationModel(DateTime.Now, "This person liked your post"),
    new NotificationModel(DateTime.Now.AddDays(-1), "This person commented your post"),
    new NotificationModel(DateTime.Now, "This person bookmarked your post"),
    new NotificationModel(DateTime.Now.AddMinutes(50), "This person liked your post"),
    new NotificationModel(DateTime.Now, "This person liked your post"),
    new NotificationModel(DateTime.Now.AddMonths(-5), "This person liked your post"),
    new NotificationModel(DateTime.Now, "This person liked your post"),
    new NotificationModel(DateTime.Now, "This person liked your post"),
    new NotificationModel(DateTime.Now, "This person liked your post"),
    new NotificationModel(DateTime.Now, "This person liked your post"),
    new NotificationModel(DateTime.Now, "This person liked your post"),
    new NotificationModel(DateTime.Now, "This person liked your post"),
    new NotificationModel(DateTime.Now, "This person liked your post"),
    new NotificationModel(DateTime.Now, "This person liked your post"),
];

        collection.ItemsSource = notifications;
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
    }

    public record NotificationModel(DateTime On , string Text)
	{
       
	}

}