using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace Munchii
{
    // TopThreeConverter class converts a list of integers to a list containing the top three integers.
    public class TopThreeConverter : IValueConverter
    {
        // Convert method takes a list of integers and returns a list containing the top three integers.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert the value to a list of integers.
            var list = value as List<int>;

            // Return the top three integers from the list.
            return list?.OrderByDescending(x => x).Take(3).ToList();
        }

        // ConvertBack method is not implemented.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
