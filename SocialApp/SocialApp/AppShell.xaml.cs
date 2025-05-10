using SocialApp.Pages;


namespace SocialApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }
        public static void RegisterRoute()
        {
            Routing.RegisterRoute("LandingPage", typeof(LandingPage));
            Routing.RegisterRoute("OnboardingPage", typeof(OnboardingPage));
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("SignUpPage", typeof(SignUpPage));
            Routing.RegisterRoute("HomePage", typeof(HomePage));

        }
    }
}
