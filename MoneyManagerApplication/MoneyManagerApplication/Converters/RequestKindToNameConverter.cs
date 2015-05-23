using System;
using System.Globalization;
using System.Windows.Data;
using MoneyManager.ViewModels.RequestManagement;

namespace MoneyManagerApplication.Converters
{
    public class RequestKindToNameConverter : IValueConverter
    {
        public static readonly RequestKindToNameConverter Instance = new RequestKindToNameConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RequestKind)
            {
                switch ((RequestKind)value)
                {
                    case RequestKind.Earning:
                        return Properties.Resources.RequestKindName_Earning;
                    case RequestKind.Expenditure:
                        return Properties.Resources.RequestKindName_Expenditure;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
