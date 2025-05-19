using SocialApp.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApp.App.Converters
{
    public class TabToPostsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProfileViewModel.ProfileTab tab && parameter is ProfileViewModel vm)
            {
                return tab switch
                {
                    ProfileViewModel.ProfileTab.MyPosts => vm.MyPosts,
                    ProfileViewModel.ProfileTab.Bookmarked => vm.BookMarkedPost,
                    ProfileViewModel.ProfileTab.Liked => vm.LikedPosts,
                    _ => vm.MyPosts
                };
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
