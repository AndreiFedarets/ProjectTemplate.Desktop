using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjectTemplate.Desktop.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public sealed class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is bool && (bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
