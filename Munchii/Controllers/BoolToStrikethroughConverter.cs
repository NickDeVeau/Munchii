using System;
using System.Globalization;
using Xamarin.Forms;

namespace Munchii
{
    public class BoolToStrikethroughConverter : IValueConverter
    {
        // Converts a boolean value to a TextDecoration value
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the value is true, return TextDecorations.Strikethrough, otherwise return TextDecorations.None
            return ((bool)value) ? TextDecorations.Strikethrough : TextDecorations.None;
        }

        // Converts back a TextDecoration value to a boolean value
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
