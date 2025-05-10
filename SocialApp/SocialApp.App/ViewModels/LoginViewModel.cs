using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refit;
using System.Net.Http;

using SocialAppLibrary.Shared.Dtos;
using SocialApp.App.Apis;
using SocialApp.App.Services;
using SocialApp.App.Pages;
using System.Text.Json;
using System.Diagnostics;
using System.Windows.Input;
//using static Java.Util.Jar.Attributes;

namespace SocialApp.App.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthApi _authApi;
    private readonly AuthService _authService;

    
    //InitPage
    private bool _rememberMe;
    public bool RememberMe
    {
        get => _rememberMe;
        set => SetProperty(ref _rememberMe, value);
    }

    public LoginViewModel(IAuthApi authApi, AuthService authService)
    {
        _authApi = authApi;
        _authService = authService;
        // ✅ Gán trạng thái RememberMe đã lưu (nếu có)
        RememberMe = Preferences.Get("RememberLogin", false);
    }

    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private string _password;

   
    //[RelayCommand]
    //private async Task LoginAsync()
    //{
    //    if (IsBusy) return;

    //    try
    //    {
    //        IsBusy = true;
    //        Debug.WriteLine("=== Bắt đầu quá trình Login ===");
            

    //        // Trước khi gọi API
    //        Debug.WriteLine("Chuẩn bị gọi API đăng ký...");

    //        var result = await _authApi.LoginAsync(new LoginDto( Email, Password));

    //        Debug.WriteLine("Kết quả từ API: " + JsonSerializer.Serialize(result));

    //        if (result != null)
    //        {
    //            // Xử lý kết quả thành công
    //            Debug.WriteLine("Đăng ký thành công!");
    //            await ShowErrorAlertAsync(result.Error ?? "Đăng ký tài khoản thành công!");


               
    //        }
    //        LoginResponse loginResponse = result.Data;
    //        _authService.Login(loginResponse);
    //        await NavigationAsync("//HomePage");
    //    }
    //    catch (HttpRequestException ex)
    //    {
    //        Debug.WriteLine($"Lỗi HTTP: {ex.Message}");
    //        Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
    //        await ToastAsync("Không thể kết nối đến server");
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($"Lỗi không xác định: {ex.Message}");
    //        Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
    //        await ToastAsync("Đã xảy ra lỗi trong quá trình đăng ký");
    //    }
    //    finally
    //    {
    //        IsBusy = false;
    //        Debug.WriteLine("=== Kết thúc quá trình đăng ký ===");
    //    }
    //}

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await ToastAsync("Vui lòng nhập đầy đủ email và mật khẩu.");
            return;
        }

        try
        {
            IsBusy = true;
            var result = await _authApi.LoginAsync(new LoginDto(Email, Password));

            if (!result.IsSuccess)
            {
                await ShowErrorAlertAsync(result.Error ?? "Đăng nhập thất bại.");
                return;
            }

            LoginResponse loginResponse = result.Data;
            _authService.Login(loginResponse);// Gọi AuthService để lưu trạng thái đăng nhập
                                              // ✅ Lưu trạng thái nếu chọn "Ghi nhớ đăng nhập"
            if (RememberMe)
            {
                Preferences.Set("RememberLogin", true);
            }
            else
            {
                Preferences.Remove("RememberLogin");
            }

            await NavigationAsync("//HomePage");    
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Lỗi: {ex.Message}\nStackTrace: {ex.StackTrace}");
            await ToastAsync("Đã xảy ra lỗi trong quá trình đăng nhập.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    
}
