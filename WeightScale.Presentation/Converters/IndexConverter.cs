﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace WeightScale.Presentation.Converters
{
    public class AddOneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DataGridRow row)
            {
                var index = row.GetIndex();
                return index + 1;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
