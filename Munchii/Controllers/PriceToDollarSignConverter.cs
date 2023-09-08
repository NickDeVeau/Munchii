using System;
using System.Globalization;
using Xamarin.Forms;

namespace Munchii.Controllers
{
    public class PriceToDollarSignConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int priceLevel)
            {
                return new string('$', priceLevel);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

