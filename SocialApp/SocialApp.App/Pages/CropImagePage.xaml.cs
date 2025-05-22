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
        if (query.TryGetValue(nameof(PhotoSource), out var photoSourceObject) && photoSourceObject is string photoSource)
        {
            if (string.IsNullOrWhiteSpace(photoSource))
            {
                // This block seems to be for when photoSource IS NOT null or whitespace,
                // but the logic inside (Toast "No photo", GoToAsync("..")) suggests it should be
                // if (string.IsNullOrWhiteSpace(photoSource))
                // Assuming the image shows the intended logic, even if it might be a bug:
                // await Toast.Make("No photo provided for cropping").Show();
                // await Shell.Current.GoToAsync("..");
                // return;
                //
                // However, if the intent is to proceed when a photoSource IS provided:
                // The original code has an empty if block for when photoSource is NOT null or whitespace,
                // and then proceeds. Let's follow the image literally.

                // The if condition is: if (!string.IsNullOrWhiteSpace(photoSource))
                // The code block inside is:
                // await Toast.Make("No photo provided for cropping").Show();
                // await Shell.Current.GoToAsync("..");
                // return;
                // This means if a photoSource *is* provided and is not empty, it shows "No photo" and navigates back.
                // This seems like a logical error in the original code, but transcribing literally:
                await Toast.Make("No photo provided for cropping").Show();
                await Shell.Current.GoToAsync("..");
                return;
            }

            // If the above if condition was string.IsNullOrWhiteSpace(photoSource)
            // then the below lines would execute if a valid photoSource was passed.
            // As it is, these lines are unreachable if photoSource has a non-whitespace value.
            // If photoSource IS null or whitespace, the outer 'if' condition is false, so these lines are also not reached.
            // This means there's a logical flaw in the code shown.
            // For the sake of accurate transcription of what's visible:
            PhotoSource = photoSource;
            imageEditor.Source = PhotoSource; // Assuming imageEditor is a XAML element or field
            imageEditor.ImageLoaded += ImageEditor_ImageLoaded; // Assuming ImageEditor_ImageLoaded is a method
        }
    }
    private void ImageEditor_ImageLoaded(object? sender, EventArgs e)
    {
        // Handle image loaded event
        // This method is a placeholder for the actual event handler logic
        imageEditor.Crop(Syncfusion.Maui.ImageEditor.ImageCropType.Circle);
        imageEditor.ImageLoaded -= ImageEditor_ImageLoaded; // Unsubscribe from the event to avoid multiple calls
    }
    // Placeholder for the event handler if it's part of this class
    // private void ImageEditor_ImageLoaded(object sender, EventArgs e)
    // {
    //     // Handle image loaded event
    // }
    private async void Cancel_Click(object sender, EventArgs e)
    {
        if (imageEditor.HasUnsavedEdits)
        {
            if (await Shell.Current.DisplayAlert("Cancel Cropping?", "Do you really want to cancel this action?", "Yes", "No"))
            {
                imageEditor.CancelEdits();
                await Shell.Current.GoToAsync("//Profile");
            }
        }
        else if (await Shell.Current.DisplayAlert("Cancel?", "Are you sure?", "Yes", "No"))
        {
            await Shell.Current.GoToAsync("//Profile");
        }
    }
    private async void AcceptChanges_Click(object sender, EventArgs e)
    {
        if (!imageEditor.HasUnsavedEdits)
        {
            await Shell.Current.DisplayAlert("Alert", "There are no changes", "OK");
            return;
        }

        imageEditor.SaveEdits();

        using var newPhotoStream = await imageEditor.GetImageStream();
        if (newPhotoStream == null)
        {
            await Shell.Current.DisplayAlert("Error", "Failed to get image stream.", "OK");
            return;
        }

        var extension = Path.GetExtension(PhotoSource);
        var tempPath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}{extension}");

        try
        {
            using var fileStream = File.OpenWrite(tempPath);
            await newPhotoStream.CopyToAsync(fileStream);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to save image: {ex.Message}", "OK");
            return;
        }

        await Shell.Current.GoToAsync($"//CropImagePage?new-src={Uri.EscapeDataString(tempPath)}");
    }

}