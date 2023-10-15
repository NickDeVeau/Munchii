using System;
using System.Globalization;
using Xamarin.Forms;

namespace Munchii
{
    // BoolToStrikethroughConverter class converts boolean values to TextDecorations.
    public class BoolToStrikethroughConverter : IValueConverter
    {
        // Convert method takes a boolean value and returns a TextDecoration.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if the value is a boolean and apply strikethrough if true.
            return value is bool isStrikethrough && isStrikethrough ? TextDecorations.Strikethrough : TextDecorations.None;
        }

        // ConvertBack method is not implemented.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
