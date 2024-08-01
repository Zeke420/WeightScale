using System;
using System.Globalization;
using System.Windows.Data;

namespace WeightScale.Presentation.Converters
{
    internal class ListViewHeightConverter : IValueConverter
    {
        public object Convert(
                object value,
                Type targetType,
                object parameter,
                CultureInfo culture)
        {
            return (double)value - double.Parse((string)parameter);
        }

        public object ConvertBack(
                object value,
                Type targetType,
                object parameter,
                CultureInfo culture)
        {
            return null;
        }
    }
}