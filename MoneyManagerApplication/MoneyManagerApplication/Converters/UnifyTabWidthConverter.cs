using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MoneyManagerApplication.Converters
{
    class UnifyTabWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var tabControl = values[0] as TabControl;
            if (tabControl == null) return DependencyProperty.UnsetValue;

            var maxWidth = double.MinValue;
            foreach (TabItem tabItem in tabControl.Items)
            {
                maxWidth = Math.Max(maxWidth, tabItem.ActualWidth);
            }

            return maxWidth;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
