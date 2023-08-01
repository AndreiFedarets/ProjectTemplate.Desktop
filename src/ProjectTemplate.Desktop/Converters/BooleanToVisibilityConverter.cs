using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProjectTemplate.Desktop.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }

        public Visibility FalseValue { get; set; }

        public BooleanToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Equals(value, TrueValue);
        }
    }
}
