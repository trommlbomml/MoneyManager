using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MoneyManagerApplication.Converters
{
    class BoolInverseConverter : IValueConverter
    {
        public static readonly BoolInverseConverter Instance = new BoolInverseConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value);
        }

        private static object Convert(object value)
        {
            return !(value is bool) ? DependencyProperty.UnsetValue : !((bool) value);
        }
    }
}
