using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApp.App.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected && parameter is string param)
            {
                if (param == "Background")
                    return isSelected ? Colors.LightBlue : Colors.Transparent;
                if (param == "Text")
                    return isSelected ? Colors.Black : Colors.Gray;

            }
            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
