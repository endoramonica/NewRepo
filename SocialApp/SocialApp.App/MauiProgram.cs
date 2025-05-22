//using AndroidX.AppCompat.View.Menu;
//using Bumptech.Glide.Load.Resource.Bitmap;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Refit;
using SocialApp.App.Services;
using SocialApp.App.Apis;
using SocialApp.App.Pages;
using SocialApp.App.ViewModels;
using System.Net.Http.Headers;
using System.Text.Json;
using SocialApp.App.Controls;
using Syncfusion.Maui.Core.Hosting;
using SocialAppLibrary.Shared;


namespace SocialApp.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiCommunityToolkit() // Đảm bảo Toolkit được sử dụng
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore() // Đảm bảo Syncfusion được sử dụng
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            
            //#24
           

            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddTransient<LoginViewModel>().AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterViewModel>().AddTransient<SignUpPage>();
            builder.Services.AddTransient<SavePostViewModel>().AddTransient<CreatePostPage>();
            builder.Services.AddTransient<DetailsViewModel>().AddTransient<PostDetailsPage>();
            builder.Services.AddTransient<ProfileViewModel>().AddTransient<Profile>();

            builder.Services.AddTransient<InitPage>();
            builder.Services.AddSingleton<IAppPreferences, AppPreferences>();
            builder.Services.AddSingleton<HomeViewModels>().AddSingleton<HomePage>();
            builder.Services.AddSingleton<RealTimeUpdatesService>();

            ConfigureRefit(builder.Services);
            return builder.Build();
        }
        private static void ConfigureRefit(IServiceCollection services)
        {
            //var baseApiUrl = "https://qjk4ssln-7175.asse.devtunnels.ms";



            //#24   lần lượt gọi đến IAuthApi + IPostApi + IUserApi
            services.AddRefitClient<IAuthApi>()
                .ConfigureHttpClient(SetHttpClient);

            services.AddRefitClient<IPostApi>(GetRefitSettings)
                .ConfigureHttpClient(SetHttpClient);

            services.AddRefitClient<IUserApi>(GetRefitSettings)
                .ConfigureHttpClient(SetHttpClient);

            void SetHttpClient(HttpClient httpClient)
            {
                httpClient.BaseAddress = new Uri(AppConstants.ApiBaseUrl);
                httpClient.Timeout = TimeSpan.FromSeconds(20); // ⏳ Đặt timeout 10 giây
                Console.WriteLine($"[Refit] BaseAddress set to: {httpClient.BaseAddress}");

              
            }

            RefitSettings GetRefitSettings(IServiceProvider sp)
            {
                var authService = sp.GetRequiredService<AuthService>();

                return new RefitSettings
                {
                    AuthorizationHeaderValueGetter = (_, __) =>
                    Task.FromResult(authService.Token ?? ""),


                };
            }

        }

    }
}