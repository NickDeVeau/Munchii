﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace Munchii
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return Color.FromHex("#D23535"); // Selected (background color)
            }
            return Color.White; // Default (background color)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

