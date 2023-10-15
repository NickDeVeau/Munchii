using System;
using System.Globalization;
using Xamarin.Forms;

namespace Munchii
{
    // BoolToColorConverter class converts boolean values to Color objects.
    public class BoolToColorConverter : IValueConverter
    {
        // Convert method takes a boolean value and returns a Color object.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if the value is a boolean and if it's true.
            if (value is bool isTrue && isTrue)
            {
                // Return red color if true.
                return Color.FromHex("#D23535");
            }

            // Return white color otherwise.
            return Color.White;
        }

        // ConvertBack method is not implemented.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
