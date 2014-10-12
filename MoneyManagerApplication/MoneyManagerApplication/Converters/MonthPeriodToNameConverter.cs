using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MoneyManager.ViewModels.RequestManagement.Regulary;

namespace MoneyManagerApplication.Converters
{
    class MonthPeriodToNameConverter : IValueConverter
    {
        public static readonly MonthPeriodToNameConverter Instance = new MonthPeriodToNameConverter(); 

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is MonthPeriod)) return DependencyProperty.UnsetValue;

            switch ((MonthPeriod) value)
            {
                case MonthPeriod.Monthly:
                    return "@Monatlich";
                case MonthPeriod.Quarterly:
                    return "@Pro Quartal";
                case MonthPeriod.HalfYearly:
                    return "@halbjährlich";
                case MonthPeriod.Yearly:
                    return "@Jährlich";
                default:
                    return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
