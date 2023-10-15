using System;
using System.Globalization;
using Xamarin.Forms;

namespace Munchii
{
    // PriceToDollarSignConverter class converts integer price levels to dollar sign strings.
    public class PriceToDollarSignConverter : IValueConverter
    {
        // Convert method takes an integer price level and returns a string of dollar signs.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert the value to an integer.
            int price = (int)value;

            // Return a string of dollar signs based on the price level.
            return new string('$', price);
        }

        // ConvertBack method is not implemented.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
