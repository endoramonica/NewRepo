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
using SocialApp.App.Models;
namespace SocialApp.App.ViewModels;
[QueryProperty(nameof(Post), nameof(Post))]
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
        var selectedPhotoSource = await ChoosePhotoAsync();
        if(!string.IsNullOrWhiteSpace(selectedPhotoSource))
        {
           PhotoPath = selectedPhotoSource;
        }
    }

    private string? _existingPhotoUrl;



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
            if (!string.IsNullOrWhiteSpace(PhotoPath) && _existingPhotoUrl != PhotoPath)
            {
                var fileName = Path.GetFileName(PhotoPath);
                var fileStream = File.OpenRead(PhotoPath);
                photoStreamPart = new StreamPart(fileStream, fileName);
            }

            // Tạo đối tượng SavePost và serialize nó
            //var serializedSavePostDto = JsonSerializer.Serialize(new SavePost
            //{
            //    Content = Content,

            //});
            var dto = new SavePost
            {
                Content = Content,
                // Nếu có ảnh, thêm đường dẫn ảnh vào DTO
                PostId = Post?.PostId ?? default
            };
            if(!string.IsNullOrWhiteSpace(_existingPhotoUrl) && string.IsNullOrWhiteSpace(PhotoPath) )
            {
                // photo existed 
                // photopath is null 
                // in case which user remove the photo
                dto.IsExistingPhotoRemoved = true;
            }
           
            var serializedSavePostDto = JsonSerializer.Serialize(dto);

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
            var savedPost = PostModel.FromDto(result.Data);
            await NavigationAsync($"//{nameof(HomePage)}",new Dictionary<string, object> { [nameof(DetailsViewModel.Post)] = savedPost});
        });
    }

    [ObservableProperty]
    private PostModel? _post;
    partial void OnPostChanged( PostModel? value)
    {
       if(value is not  null)
        {
            Content = Post.Content;
            PhotoPath = Post.PhotoUrl ?? "";
           _existingPhotoUrl = Post.PhotoUrl;
        }
    }
    
}

