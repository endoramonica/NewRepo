
using Microsoft.Maui.Controls;

namespace SocialApp.App.Pages
{
    public partial class PostDetailsPage : ContentPage
    {
        public PostDetailsPage()
        {
            InitializeComponent();
        }


        private void OnCommentSubmitted(object sender, EventArgs e)
        {
            if (CommentEntry == null || comments == null)
            {
                DisplayAlert("Lỗi", "Không tìm thấy phần tử cần thiết", "OK");
                return;
            }

            string comment = CommentEntry.Text;

            if (!string.IsNullOrWhiteSpace(comment))
            {
                comments.Children.Add(new Label
                {
                    Text = comment,
                    TextColor = Colors.Black,
                    FontSize = 14
                });

                DisplayAlert("Thông báo", $"Bạn đã gửi bình luận: {comment}", "OK");

                CommentEntry.Text = string.Empty;
            }
            else
            {
                DisplayAlert("Lỗi", "Vui lòng nhập bình luận trước khi gửi!", "OK");
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("//HomePage");


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }

        }

    }
}
