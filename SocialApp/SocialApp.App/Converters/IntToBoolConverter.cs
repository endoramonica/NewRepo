using System.Collections;
using System.Globalization;

namespace SocialApp.App.Converters;

public class IntToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            return intValue > 0;
        }

        // Nếu là Collection Count
        if (value is ICollection collection)
        {
            return collection.Count > 0;
        }

        // Nếu là string number
        if (value is string stringValue && int.TryParse(stringValue, out int parsedValue))
        {
            return parsedValue > 0;
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? 1 : 0;
        }

        return 0;
    }
}