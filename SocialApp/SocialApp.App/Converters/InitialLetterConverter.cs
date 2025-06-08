using System.Globalization;
using System.Text;

namespace SocialApp.App.Converters;

public class InitialLetterConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string name && !string.IsNullOrWhiteSpace(name))
        {
            // Lấy chữ cái đầu tiên và chuyển thành uppercase
            var firstChar = name.Trim().First().ToString().ToUpper();

            // Xử lý các ký tự đặc biệt của tiếng Việt
            return RemoveVietnameseDiacritics(firstChar);
        }

        return "?"; // Default nếu không có tên
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not supported for InitialLetterConverter");
    }

    /// <summary>
    /// Loại bỏ dấu tiếng Việt để hiển thị chữ cái đầu
    /// </summary>
    private static string RemoveVietnameseDiacritics(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}