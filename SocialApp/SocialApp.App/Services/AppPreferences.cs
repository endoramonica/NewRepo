
// Services/AppPreferences.cs
using Microsoft.Maui.Storage;

namespace SocialApp.App.Services
{
    public class AppPreferences : IAppPreferences
    {
        public void SetBool(string key, bool value)
        {
            Preferences.Set(key, value);
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            return Preferences.Get(key, defaultValue);
        }

        public void Remove(string key)
        {
            Preferences.Remove(key);
        }
    }
}