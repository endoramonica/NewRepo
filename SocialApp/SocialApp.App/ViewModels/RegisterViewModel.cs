using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SocialAppLibrary.Shared.Dtos;
using SocialApp.App.Apis;
using SocialApp.App.Pages;
using System.Diagnostics;
using System.Text.Json;
namespace SocialApp.App.ViewModels;

public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthApi _authApi;

        public RegisterViewModel(IAuthApi authApi)
        {
            _authApi = authApi;
        }

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Debug.WriteLine("=== Bắt đầu quá trình đăng ký ===");
                Debug.WriteLine($"Email: {Email}");
                Debug.WriteLine($"Name: {Name}");
                
                // Trước khi gọi API
                Debug.WriteLine("Chuẩn bị gọi API đăng ký...");
                
                var result = await _authApi.RegisterAsync(new RegisterDto(Name, Email, Password));

                Debug.WriteLine("Kết quả từ API: " + JsonSerializer.Serialize(result));

                if (result != null)
                {
                    // Xử lý kết quả thành công
                    Debug.WriteLine("Đăng ký thành công!");
                     await ShowErrorAlertAsync(result.Error ?? "Đăng ký tài khoản thành công!");

   
                    await NavigationAsync("//loginPage");
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Lỗi HTTP: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                await ToastAsync( "Không thể kết nối đến server");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi không xác định: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                await ToastAsync( "Đã xảy ra lỗi trong quá trình đăng ký");
            }
            finally
            {
                IsBusy = false;
                Debug.WriteLine("=== Kết thúc quá trình đăng ký ===");
            }
        }
    }

