
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refit;
using SocialApp.App.Apis;
using SocialApp.App.Pages;
using SocialAppLibrary.Shared.Dtos;

namespace SocialApp.App.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    protected async Task ShowErrorAlertAsync(string message) =>
        await Shell.Current.DisplayAlert("Error", message, "OK");

    protected async Task NavigationAsync(string url) =>
        await Shell.Current.GoToAsync(url, animate: true);
    protected async Task NavigationAsync(string url, Dictionary<string, object> parameters) =>
        await Shell.Current.GoToAsync(url, animate: true, parameters);
    protected async Task NavigateBackAsync() => await NavigationAsync("..");

    protected async Task ToastAsync(string message)
    {
        await Toast.Make(message).Show();
    }

    protected async Task MakeApiCall(Func<Task> apiCall)
    {
        IsBusy = true;
        try
        {
            await apiCall.Invoke();
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"API Error: {ex.StatusCode}, {ex.Message}, {ex.Content}");
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (await Shell.Current.DisplayAlert("Login Expired!", "Login has expired. Back to login page?", "Yes", "No"))
                {
                    await NavigationAsync($"//{nameof(LoginPage)}");
                }
            }
            else
            {
                await ShowErrorAlertAsync(ex.Message);
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAlertAsync(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected async Task<string?> ChoosePhotoAsync()
    {
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            await ShowErrorAlertAsync("Thiết bị không hỗ trợ chọn ảnh.");
            return null;
        }

        const string PickFromDevice = "Chọn từ thiết bị";
        const string CapturePhoto = "Chụp ảnh";

        var result = await Shell.Current.DisplayActionSheet(
            "Chọn ảnh",
            "Hủy",
            null,
            PickFromDevice,
            CapturePhoto);

        if (string.IsNullOrWhiteSpace(result))
            return null;

        switch (result)
        {
            case PickFromDevice:
                return await PickPhotoFromDeviceAsync();
            case CapturePhoto:
                return await CapturePhotoAsync();
            default:
                return null;
        }

        async Task<string?> PickPhotoFromDeviceAsync()
        {
            try
            {
                FileResult? fileResult = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Chọn ảnh"
                });

                if (fileResult is null)
                {
                    await ToastAsync("Không có ảnh nào được chọn.");
                    return null;
                }

                return fileResult.FullPath;
            }
            catch (Exception ex)
            {
                await ShowErrorAlertAsync($"Không thể chọn ảnh: {ex.Message}");
                return null;
            }
        }

        async Task<string?> CapturePhotoAsync()
        {
            try
            {
                var cameraPermissionStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (cameraPermissionStatus != PermissionStatus.Granted)
                    cameraPermissionStatus = await Permissions.RequestAsync<Permissions.Camera>();

                var storagePermissionStatus = await Permissions.CheckStatusAsync<Permissions.StorageWrite>(); // nên dùng StorageWrite
                if (storagePermissionStatus != PermissionStatus.Granted)
                    storagePermissionStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();

                if (cameraPermissionStatus == PermissionStatus.Denied || storagePermissionStatus == PermissionStatus.Denied)
                {
                    await ShowErrorAlertAsync("Bạn cần cấp quyền trong phần cài đặt ứng dụng.");
                    return null;
                }

                FileResult? photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Chụp ảnh"
                });

                if (photo is null)
                {
                    await ShowErrorAlertAsync("Không có ảnh nào được chụp.");
                    return null;
                }

                return photo.FullPath;
            }
            catch (Exception ex)
            {
                await ShowErrorAlertAsync($"Không thể chụp ảnh: {ex.Message}");
                return null;
            }
        }
    }


}