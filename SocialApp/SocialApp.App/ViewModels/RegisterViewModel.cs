using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SocialAppLibrary.Shared.Dtos;
using SocialApp.App.Apis;
using SocialApp.App.Pages;
using System.Diagnostics;
using System.Text.Json;
using Refit;
namespace SocialApp.App.ViewModels;
[QueryProperty(nameof(CropPhotoSource), "new-src")]
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
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await ToastAsync("Vui lòng nhập đầy đủ thông tin");
            return;
        }

        await MakeApiCall(async () =>
        {
            var registerDto = new RegisterDto(Name, Email, Password);
            var result = await _authApi.RegisterAsync(registerDto);

            if (!result.IsSuccess)
            {
                await ShowErrorAlertAsync(result.Error);
                return;
            }

            var userId = result.Data; // Hoặc result.Data.Id nếu là UserDto

            if (!string.IsNullOrWhiteSpace(PhotoImageSource) && PhotoImageSource != "account_circle.png")
            {
                var photoName = Path.GetFileName(PhotoImageSource);
                using var fs = File.OpenRead(PhotoImageSource);
                var photoStreamPart = new StreamPart(fs, photoName);

                var apiResult = await _authApi.UploadPhotoAsync(userId, photoStreamPart);
                if (!result.IsSuccess)
                {
                   
                    await ToastAsync("Đã xảy ra lỗi trong quá trình tải ảnh lên");
                    return;
                }

                await ToastAsync("Tải ảnh lên thành công");
            }

            await ToastAsync("Đăng ký thành công");
            await NavigationAsync("//LoginPage");
        });
    }


    [ObservableProperty]
    private string _photoImageSource = "add_a_photo.png";
    [ObservableProperty]
    private string? _cropPhotoSource = "account_circle.png";

    async partial void OnCropPhotoSourceChanged(string? oldValue, string? newValue)
    {
        if (!string.IsNullOrWhiteSpace(newValue))
        {
            PhotoImageSource = newValue;


        }
    }

    [RelayCommand]
    private async Task ChangePhotoAsync()
    {
        var selectedPhotoSource = await ChoosePhotoAsync();
        if (!string.IsNullOrWhiteSpace(selectedPhotoSource))
        {
            var param = new Dictionary<string, object>
            {
                [nameof(CropImagePage.PhotoSource)] = selectedPhotoSource
            };
            await Shell.Current.GoToAsync("//CropImagePage", param);
        }
    }
    [RelayCommand]
    private async Task AcessLogin()
    {
        await Shell.Current.GoToAsync("///LoginPage");
    }
}
