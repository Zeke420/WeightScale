﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WeightScale.Presentation.Converters
{
    public class PassFailToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool passed)
            {
                return passed ? Brushes.Green : Brushes.Red;
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}