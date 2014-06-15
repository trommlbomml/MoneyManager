using System;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.RequestManagement
{
    public class RequestDialogViewModel : ViewModel
    {
        private string _description;
        private double _value;
        private DateTime _date;
        private DateTime _firstPossibleDate;
        private DateTime _lastPossibleDate;
        private string _dateAsString;

        public RequestDialogViewModel(int year, int monthIndex, Action<RequestDialogViewModel> onOk)
        {
            var month = monthIndex+1;
            Date = new DateTime(year, month, 1);
            FirstPossibleDate = new DateTime(year, month, 1);
            LastPossibleDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            CreateRequestCommand = new CommandViewModel(() => onOk(this));

            UpdateLocalizedProperties();
        }

        public CommandViewModel CreateRequestCommand { get; private set; }

        public string Description
        {
            get { return _description; }
            set { SetBackingField("Description", ref _description, value); }
        }

        public double Value
        {
            get { return _value; }
            set { SetBackingField("Value", ref _value, value); }
        }

        public DateTime Date
        {
            get { return _date; }
            set { SetBackingField("Date", ref _date, value, o => UpdateLocalizedProperties()); }
        }

        private void UpdateLocalizedProperties()
        {
            DateAsString = string.Format(Properties.Resources.RequestDayOfMonthFormat, Date);
        }

        public string DateAsString
        {
            get { return _dateAsString; }
            set { SetBackingField("DateAsString", ref _dateAsString, value); }
        }

        public DateTime FirstPossibleDate
        {
            get { return _firstPossibleDate; }
            set { SetBackingField("FirstPossibleDate", ref _firstPossibleDate, value); }
        }

        public DateTime LastPossibleDate
        {
            get { return _lastPossibleDate; }
            set { SetBackingField("LastPossibleDate", ref _lastPossibleDate, value); }
        }
    }
}
