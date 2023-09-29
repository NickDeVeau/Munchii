using System;
using System.Globalization;
using Xamarin.Forms;

namespace Munchii
{
    public class PriceToDollarSignConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int priceLevel && priceLevel > 0)
            {
                return new string('$', priceLevel);
            }
            return null;  // Return null if the priceLevel is invalid
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
