using System;
using System.Globalization;
using System.Windows.Data;

namespace WeightScale.Presentation.Converters
{
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return false;
            }

            if (!( value is bool ))
            {
                throw new InvalidCastException("value has to be bool");
            }

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}