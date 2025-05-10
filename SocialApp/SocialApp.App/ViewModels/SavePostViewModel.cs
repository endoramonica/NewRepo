using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refit;
using SocialApp.App.Apis;
using SocialAppLibrary.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SkiaSharp;
using SocialApp.App.Services;
using SocialApp.App.Pages;
namespace SocialApp.App.ViewModels
{
    public partial class SavePostViewModel : BaseViewModel
    {
        private readonly IAuthApi _authApi;
        private readonly AuthService _authService;
        private readonly IPostApi _postApi;
        public SavePostViewModel(IPostApi postApi, IAuthApi authApi, AuthService authService)
        {
            _postApi = postApi;
            _authApi = authApi;
            _authService = authService;
        }
        [ObservableProperty]
        public string _content = string.Empty;
        [ObservableProperty]
        public string _photoPath = string.Empty;


        [RelayCommand]
        private async Task SelectPhotoAsync()
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await ShowErrorAlertAsync( "Thiết bị không hỗ trợ chọn ảnh.");
                return;
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
                return;

            switch (result)
            {
                case PickFromDevice:
                    await PickPhotoFromDeviceAsync();
                    break;

                case CapturePhoto:
                    await CapturePhotoAsync();
                    break;

                default:
                    // Người dùng chọn "Hủy" hoặc đóng prompt
                    break;
            }


            async Task PickPhotoFromDeviceAsync()
            {
                try
                {
                    FileResult? fileResult = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                    {
                        Title = "Select photos"
                    });

                    if (fileResult is null)
                    {
                        await ToastAsync("no photo selected");
                        return;
                    }// Lưu đường dẫn của ảnh đã chọn
                    PhotoPath = fileResult.FullPath;
                }
                catch (Exception ex)
                {
                    await ShowErrorAlertAsync( $"Không thể chọn ảnh: {ex.Message}");
                }

            }

            async Task CapturePhotoAsync()
            {
                try
                {
                    // Yêu cầu quyền camera và lưu trữ
                    var cameraPermissionStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
                    if (cameraPermissionStatus != PermissionStatus.Granted)
                    {
                        cameraPermissionStatus = await Permissions.RequestAsync<Permissions.Camera>();
                    }

                    var storagePermissionStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                    if (storagePermissionStatus != PermissionStatus.Granted)
                    {
                        storagePermissionStatus = await Permissions.RequestAsync<Permissions.StorageRead>();
                    }

                    // Nếu quyền bị từ chối, thông báo cho người dùng
                    if (cameraPermissionStatus == PermissionStatus.Denied || storagePermissionStatus == PermissionStatus.Denied)
                    {
                        await ShowErrorAlertAsync("You can enable permissions in the app settings.");
                        return; // Dừng lại nếu quyền bị từ chối
                    }

                    // Chụp ảnh
                    FileResult? photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
                    {
                        Title = "Chụp ảnh"
                    });

                    if (photo is null)
                    {
                        await ShowErrorAlertAsync("Không có ảnh nào được chụp");
                        return;
                    }

                    // Lưu đường dẫn của ảnh đã chụp
                    PhotoPath = photo.FullPath;
                }
                catch (Exception ex)
                {
                    await ShowErrorAlertAsync($"Không thể chụp ảnh: {ex.Message}");
                }
            }

        }




        [RelayCommand]
        private void RemovePhotoAsync()
        {
            // Logic xóa ảnh (ví dụ: xóa đường dẫn ảnh)
            PhotoPath = "";

        }
        // Thực hiện INotifyPropertyChanged để cập nhật UI
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Phương thức nén ảnh bằng SkiaSharp
        private async Task<string> CompressImageAsync(string imagePath, int quality = 80)
        {
            try
            {
                // Đọc file ảnh gốc
                using var inputStream = File.OpenRead(imagePath);
                using var originalBitmap = SKBitmap.Decode(inputStream);

                // Tạo thông tin ảnh và surface để vẽ
                var imageInfo = new SKImageInfo(originalBitmap.Width, originalBitmap.Height);
                using var surface = SKSurface.Create(imageInfo);
                using var canvas = surface.Canvas;
                canvas.DrawBitmap(originalBitmap, 0, 0);

                // Nén ảnh với chất lượng được chỉ định
                using var image = surface.Snapshot();
                using var data = image.Encode(SKEncodedImageFormat.Jpeg, quality);

                // Lưu ảnh đã nén vào file tạm
                var compressedFilePath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}.jpg");
                await File.WriteAllBytesAsync(compressedFilePath, data.ToArray());

                return compressedFilePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error compressing image: {ex.Message}");
            }
        }

        // phương thức dùng để lưu bài đăng + xử lí  nén ảnh bên trong 
        //[RelayCommand]
        //private async Task SavePostAsync()
        //{
        //    if (string.IsNullOrWhiteSpace(Content) && string.IsNullOrWhiteSpace(PhotoPath))
        //    {
        //        await ToastAsync("Either content or photo is required");
        //        return;
        //    }

        //    await MakeApiCall(async () =>
        //    {

        //        StreamPart? photoStreamPart = null;
        //        //FileStream? fileStream = null;
        //        //string? compressedFilePath = null;


        //        // Nếu có ảnh, nén ảnh trước khi gửi
        //        if (!string.IsNullOrWhiteSpace(PhotoPath))
        //        {
        //            var fileName = Path.GetFileName(PhotoPath);
        //            var fileStream = File.OpenRead(PhotoPath);
        //            photoStreamPart = new StreamPart(fileStream, fileName);
        //        }

        //        // Gọi API SavePostAsync để lưu bài viết
        //        var result = await _postApi.SavePostAsync(photoStreamPart, serializedSavePostDto);
        //        var serializedSavePostDto = JsonSerializer.Serialize(new SavePost { Content = Content });

        //        // Đảm bảo token được thêm vào header trước khi gọi API
        //        // Cách làm này phụ thuộc vào cách bạn đã thiết lập Refit
        //        // Nếu bạn đang sử dụng DI để cấu hình Refit, bạn có thể bỏ qua bước này

        //        //var result = await _postApi.SavePostAsync(photoStreamPart, serializedSavePostDto);
        //        if (!result.IsSuccess)
        //        {

        //            await ShowErrorAlertAsync(result.Error);
        //            return;

        //        }

        //        await ToastAsync("Post Saved!");
        //        Content = null;
        //        PhotoPath = "";
        //        await NavigateBackAsync();

        //    });
        //}


        [RelayCommand]
        private async Task SavePostAsync()
        {
            if (string.IsNullOrWhiteSpace(Content) && string.IsNullOrWhiteSpace(PhotoPath))
            {
                await ToastAsync("Either content or photo is required");
                return;
            }

            await MakeApiCall(async () =>
            {
                StreamPart? photoStreamPart = null;

                // Nếu có ảnh, mở và tạo StreamPart
                if (!string.IsNullOrWhiteSpace(PhotoPath))
                {
                    var fileName = Path.GetFileName(PhotoPath);
                    var fileStream = File.OpenRead(PhotoPath);
                    photoStreamPart = new StreamPart(fileStream, fileName);
                }

                // Tạo đối tượng SavePost và serialize nó
                var serializedSavePostDto = JsonSerializer.Serialize(new SavePost
                {
                    Content = Content,
                    
                });


                // Gọi API SavePostAsync để lưu bài viết
                var result = await _postApi.SavePostAsync(photoStreamPart, serializedSavePostDto);

                if (!result.IsSuccess)
                {
                    await ShowErrorAlertAsync(result.Error);
                    return;
                }

                await ToastAsync("🎉 Bài viết đã được đăng thành công!");

                Content = null;
                PhotoPath = "";
                await NavigationAsync($"//{nameof(HomePage)}");
            });
        }


    }

}
    
