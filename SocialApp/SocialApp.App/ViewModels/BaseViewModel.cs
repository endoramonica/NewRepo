using CommunityToolkit.Mvvm.ComponentModel;
using Refit;
using SocialApp.App.Pages;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace SocialApp.App.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    protected async Task ShowErrorAlertAsync(string message) =>
        await Shell.Current.DisplayAlert("Error", message, "OK");

    protected async Task NavigationAsync(string url) =>
        await Shell.Current.GoToAsync(url, animate: true);
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
}

