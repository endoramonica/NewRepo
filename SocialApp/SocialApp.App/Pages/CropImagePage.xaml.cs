using Syncfusion.Maui.ImageEditor;
using CommunityToolkit.Maui.Alerts;

namespace SocialApp.App.Pages;

[QueryProperty(nameof(PhotoSource), nameof(PhotoSource))]
public partial class CropImagePage : ContentPage, IQueryAttributable
{
    public CropImagePage()
    {
        InitializeComponent();
    }

    public string PhotoSource { get; set; }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Console.WriteLine("[DEBUG] ApplyQueryAttributes called");
        if (query.TryGetValue(nameof(PhotoSource), out var photoSourceObject)
            && photoSourceObject is string photoSource)
        {
            if (string.IsNullOrWhiteSpace(photoSource))
            {
                Console.WriteLine("[ERROR] No photo provided for cropping");
                await Toast.Make("Không có ảnh được cung cấp để cắt").Show();
                await Shell.Current.GoToAsync("//Profile");
                return;
            }

            PhotoSource = photoSource;
            imageEditor.Source = PhotoSource;
            imageEditor.ImageLoaded += ImageEditor_ImageLoaded;
            Console.WriteLine($"[DEBUG] PhotoSource set to: {PhotoSource}");
        }
        else
        {
            Console.WriteLine("[ERROR] Invalid or missing PhotoSource in query");
            await Toast.Make("Dữ liệu ảnh không hợp lệ").Show();
            await Shell.Current.GoToAsync("//Profile");
        }
    }

    private async void ImageEditor_ImageLoaded(object? sender, EventArgs e)
    {
        try
        {
            Console.WriteLine("[DEBUG] ImageEditor_ImageLoaded triggered");
            imageEditor.ImageLoaded -= ImageEditor_ImageLoaded;

            await Task.Delay(100);
            imageEditor.Crop(ImageCropType.Circle);
            Console.WriteLine($"[DEBUG] After Crop - HasUnsavedEdits: {imageEditor.HasUnsavedEdits}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] ImageEditor_ImageLoaded failed: {ex.Message}");
            await Shell.Current.DisplayAlert("Lỗi", $"Không thể cắt ảnh: {ex.Message}", "OK");
        }
    }

    private async void Cancel_Click(object sender, EventArgs e)
    {
        try
        {
            Console.WriteLine("[DEBUG] Cancel_Click triggered");
            if (imageEditor.HasUnsavedEdits)
            {
                if (await Shell.Current.DisplayAlert("Hủy cắt ảnh?", "Bạn có muốn hủy hành động này?", "Có", "Không"))
                {
                    imageEditor.CancelEdits();
                    await Shell.Current.GoToAsync("//Profile");
                }
            }
            else
            {
                if (await Shell.Current.DisplayAlert("Hủy?", "Bạn có chắc chắn?", "Có", "Không"))
                {
                    await Shell.Current.GoToAsync("//Profile");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Cancel_Click failed: {ex.Message}");
            await Shell.Current.GoToAsync("//Profile");
        }
    }

    private async void AcceptChanges_Click(object sender, EventArgs e)
    {
        try
        {
            Console.WriteLine("[DEBUG] AcceptChanges_Click triggered");
            if (!imageEditor.HasUnsavedEdits)
            {
                Console.WriteLine("[DEBUG] No unsaved edits");
                await Shell.Current.DisplayAlert("Thông báo", "Không có thay đổi để lưu", "OK");
                return;
            }

            imageEditor.SaveEdits();
            Console.WriteLine("[DEBUG] SaveEdits called");

            var newPhotoStream = await imageEditor.GetImageStream();
            if (newPhotoStream == null)
            {
                Console.WriteLine("[ERROR] GetImageStream returned null");
                await Shell.Current.DisplayAlert("Lỗi", "Không thể lấy luồng ảnh.", "OK");
                return;
            }

            var extension = Path.GetExtension(PhotoSource) ?? ".jpg";
            var fileName = $"cropped_{Guid.NewGuid()}{extension}";
            var tempPath = Path.Combine(FileSystem.CacheDirectory, fileName);

            Console.WriteLine($"[DEBUG] Saving cropped image to: {tempPath}");
            using (var fileStream = File.Create(tempPath))
            {
                await newPhotoStream.CopyToAsync(fileStream);
            }

            newPhotoStream?.Dispose();
            Console.WriteLine("[DEBUG] Image stream disposed");

            var encodedPath = Uri.EscapeDataString(tempPath);
            // Sửa tham số truy vấn từ CroppedImage sang new-src
            await Shell.Current.GoToAsync($"//Profile?new-src={encodedPath}");
            Console.WriteLine($"[DEBUG] Navigating to Profile with new-src={encodedPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] AcceptChanges_Click failed: {ex.Message}");
            await Shell.Current.DisplayAlert("Lỗi", $"Không thể lưu ảnh đã cắt: {ex.Message}", "OK");
        }
    }

    protected override void OnDisappearing()
    {
        if (imageEditor != null)
        {
            imageEditor.ImageLoaded -= ImageEditor_ImageLoaded;
        }
        base.OnDisappearing();
        Console.WriteLine("[DEBUG] CropImagePage OnDisappearing");
    }
}