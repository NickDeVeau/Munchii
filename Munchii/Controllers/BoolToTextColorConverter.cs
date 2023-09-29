using System;
using System.Globalization;
using Xamarin.Forms;

namespace Munchii
{
    public class BoolToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return Color.White; // Selected (text color)
            }
            return Color.Black; // Default (text color)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

