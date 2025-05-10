namespace SocialApp.App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // 🛠️ Quan trọng: Sử dụng AppShell thay vì chỉ một Page
            MainPage = new AppShell();

        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    return new Window(new AppShell());
        //}
        

    }
}
