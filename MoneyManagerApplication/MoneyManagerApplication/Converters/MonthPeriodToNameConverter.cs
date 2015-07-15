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
                    return Properties.Resources.PeriodName_Monthly;
                case MonthPeriod.TwoMonthly:
                    return Properties.Resources.PeriodName_TwoMonthly;
                case MonthPeriod.Quarterly:
                    return Properties.Resources.PeriodName_Quarterly;
                case MonthPeriod.HalfYearly:
                    return Properties.Resources.PeriodName_HalfYearly;
                case MonthPeriod.Yearly:
                    return Properties.Resources.PeriodName_Yearly;
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
