using System;
using Munchii.Models;
using System.Globalization;
using Xamarin.Forms;

namespace Munchii
{
    public class TopThreeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Place item && item != null)
            {
                if (item.IsTopThree)
                    return Color.FromHex("#D23535"); // For top 3
                else
                    return Color.Default; // For the rest
            }
            return Color.Default; // Default value
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}

