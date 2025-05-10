using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Services/IAppPreferences.cs
namespace SocialApp.App.Services
{
    public interface IAppPreferences
    {
        void SetBool(string key, bool value);
        bool GetBool(string key, bool defaultValue = false);
        void Remove(string key);
    }
}

