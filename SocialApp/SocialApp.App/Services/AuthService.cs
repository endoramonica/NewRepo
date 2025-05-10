//using GoogleGson.Annotations;
using SocialAppLibrary.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http.Json;
using SocialApp.App.ViewModels;
using SocialApp.App.Pages;

//#25
namespace SocialApp.App.Services
{
    public class AuthService : BaseViewModel
    {
        private const string UserDataKey = "udata";
        public string? Token { get; set; }
        public LoggedInUser? User { get; set; }

        public bool IsLoggedIn => User is not null && User.ID != default && !string.IsNullOrWhiteSpace(Token);

        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Không cần Task.Run() vì đây là các thao tác IO-bound
        public void Login(LoginResponse loginResponse)
        {
            (User, Token) = loginResponse;
            Preferences.Default.Set(UserDataKey, JsonSerializer.Serialize(loginResponse));
           
        }

        public void Logout()
        {
            (User, Token) = (null, null);
            Preferences.Default.Remove(UserDataKey);
        }

        // Không cần Task.Run() trong Initialize()
        public void Initialize()
        {
            var udata = Preferences.Default.Get<string?>(UserDataKey, null);
            if (!string.IsNullOrWhiteSpace(udata))
            {
                try
                {
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(udata);
                    if (loginResponse != null && loginResponse.User is not null && loginResponse.User.ID != default)
                    {
                        (User, Token) = loginResponse;
                    }
                    else
                    {
                        Preferences.Default.Remove(UserDataKey);
                    }
                }
                catch (JsonException ex)
                {
                    // Xử lý lỗi khi deserialize
                    Debug.WriteLine($"Error deserializing user data: {ex.Message}");
                    Preferences.Default.Remove(UserDataKey);
                }
            }
        }

        
        

    }

}
