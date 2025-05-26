
using SocialApp.App.Pages;


namespace SocialApp.App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }
        public static void RegisterRoute()
        {
            //Routing.RegisterRoute(nameof(PostDetailsPage), typeof(PostDetailsPage));
            //Routing.RegisterRoute(nameof(CreatePostPage), typeof(CreatePostPage));
            Routing.RegisterRoute(nameof(LandingPage), typeof(LandingPage));
            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));

            Routing.RegisterRoute(nameof(PostDetailsPage), typeof(PostDetailsPage));
            Routing.RegisterRoute(nameof(CreatePostPage), typeof(CreatePostPage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(Profile), typeof(Profile));
            Routing.RegisterRoute(nameof(FollowPage), typeof(FollowPage));
            Routing.RegisterRoute(nameof(CropImagePage), typeof(CropImagePage));


        }
    }
}
