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
        if (query.TryGetValue(nameof(PhotoSource), out var photoSourceObject)
            && photoSourceObject is string photoSource)
        {
            // Fixed logic: Check if photoSource is null or empty
            if (string.IsNullOrWhiteSpace(photoSource))
            {
                await Toast.Make("No photo provided for cropping").Show();
                await Shell.Current.GoToAsync("//Profile");
                return;
            }

            PhotoSource = photoSource;
            imageEditor.Source = PhotoSource;
            imageEditor.ImageLoaded += ImageEditor_ImageLoaded;
        }
    }

    private async void ImageEditor_ImageLoaded(object? sender, EventArgs e)
    {
        try
        {
            // Unsubscribe to prevent multiple calls
            imageEditor.ImageLoaded -= ImageEditor_ImageLoaded;

            // Wait a bit for the image to be fully rendered
            await Task.Delay(100);

            // Apply circle crop
            imageEditor.Crop(ImageCropType.Circle);

            // Don't call SaveEdits() immediately - let user see the crop preview
            Console.WriteLine($"After Crop - HasUnsavedEdits: {imageEditor.HasUnsavedEdits}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ImageEditor_ImageLoaded: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", $"Failed to crop image: {ex.Message}", "OK");
        }
    }

    private async void Cancel_Click(object sender, EventArgs e)
    {
        try
        {
            if (imageEditor.HasUnsavedEdits)
            {
                if (await Shell.Current.DisplayAlert("Cancel Cropping?", "Do you really want to cancel this action?", "Yes", "No"))
                {
                    imageEditor.CancelEdits();
                    await Shell.Current.GoToAsync("//Profile");
                }
            }
            else
            {
                if (await Shell.Current.DisplayAlert("Cancel?", "Are you sure?", "Yes", "No"))
                {
                    await Shell.Current.GoToAsync("//Profile");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Cancel_Click: {ex.Message}");
            await Shell.Current.GoToAsync("//Profile");
        }
    }

    private async void AcceptChanges_Click(object sender, EventArgs e)
    {
        try
        {
            if (!imageEditor.HasUnsavedEdits)
            {
                Console.WriteLine($"HasUnsavedEdits: {imageEditor.HasUnsavedEdits}");
                await Shell.Current.DisplayAlert("Alert", "There are no changes to save", "OK");
                return;
            }

            // Save the edits
            imageEditor.SaveEdits();

            // Get the edited image stream
            var newPhotoStream = await imageEditor.GetImageStream();
            if (newPhotoStream == null)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to get image stream.", "OK");
                return;
            }

            // Generate unique filename
            var extension = Path.GetExtension(PhotoSource) ?? ".jpg";
            var fileName = $"cropped_{Guid.NewGuid()}{extension}";
            var tempPath = Path.Combine(FileSystem.CacheDirectory, fileName);

            // Save the cropped image
            using (var fileStream = File.Create(tempPath))
            {
                await newPhotoStream.CopyToAsync(fileStream);
            }

            // Dispose the stream
            newPhotoStream?.Dispose();

            // Navigate back with the new image path
            var encodedPath = Uri.EscapeDataString(tempPath);
            await Shell.Current.GoToAsync($"//Profile?CroppedImage={encodedPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AcceptChanges_Click: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", $"Failed to save cropped image: {ex.Message}", "OK");
        }
    }

    // Clean up when page is disposed
    protected override void OnDisappearing()
    {
        if (imageEditor != null)
        {
            imageEditor.ImageLoaded -= ImageEditor_ImageLoaded;
        }
        base.OnDisappearing();
    }
}