using System;
using System.Globalization;
using Xamarin.Forms;

namespace Munchii
{
    // BoolToTextColorConverter class converts boolean values to Color objects for text.
    public class BoolToTextColorConverter : IValueConverter
    {
        // Convert method takes a boolean value and returns a Color object for text.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if the value is a boolean and if it's true.
            if (value is bool isWhite && isWhite)
            {
                // Return white color for text if true.
                return Color.White;
            }

            // Return black color for text otherwise.
            return Color.Black;
        }

        // ConvertBack method is not implemented.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
