using SocialApp.App.ViewModels;
using SocialAppLibrary.Shared.Dtos.ChatDto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApp.App.Converters
{
    public class FriendActionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserDto user && App.Current.MainPage.BindingContext is FriendViewModel viewModel)
            {
                return new FriendActionDto(viewModel.CurrentUserId, user.Id);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Handle string input from FollowViewModel.GetFollowText
            if (value is string stringValue && parameter is string param)
            {
                if (param == "ButtonBackground")
                {
                    return stringValue switch
                    {
                        "Follow" => Colors.Blue,
                        "Following" => Colors.Gray,
                        _ => Colors.Transparent
                    };
                }
            }

            // Keep existing logic for bool inputs to avoid breaking other uses
            if (value is bool isSelected && parameter is string param2)
            {
                if (param2 == "Background")
                    return isSelected ? Colors.LightBlue : Colors.Transparent;
                if (param2 == "Text")
                    return isSelected ? Colors.Black : Colors.Gray;
            }

            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}