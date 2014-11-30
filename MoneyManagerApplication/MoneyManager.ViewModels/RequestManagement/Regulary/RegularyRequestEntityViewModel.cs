using System;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.RequestManagement.Regulary
{
    public class StandingOrderEntityViewModel : EntityViewModel
    {
        private string _description;
        private string _category;
        private int _monthPeriod;
        private double _value;
        private string _valueAsString;
        private string _monthPeriodAsString;

        public StandingOrderEntityViewModel(ApplicationViewModel application, string entityId) : base(application, entityId)
        {
        }

        public override void Refresh()
        {
            var request = Application.Repository.QueryStandingOrder(EntityId);
            Description = request.Description;
            Category = request.Category != null ? request.Category.Name : Properties.Resources.NoCategory;
            Value = request.Value;
            MonthPeriod = request.MonthPeriodStep;
        }

        public string Description
        {
            get { return _description; }
            set { SetBackingField("Description", ref _description, value); }
        }

        public string Category
        {
            get { return _category; }
            set { SetBackingField("Category", ref _category, value); }
        }

        public double Value
        {
            get { return _value; }
            set { SetBackingField("Value", ref _value, value, o => ValueAsString = string.Format(Properties.Resources.MoneyValueFormat, Value)); }
        }

        public int MonthPeriod
        {
            get { return _monthPeriod; }
            set { SetBackingField("MonthPeroid", ref _monthPeriod, value, o => UpdateMonthPeriodAsString()); }
        }

        private void UpdateMonthPeriodAsString()
        {
            switch (MonthPeriod)
            {
                case 1:
                    MonthPeriodAsString = Properties.Resources.OncePerMonth;
                    break;
                case 3:
                    MonthPeriodAsString = Properties.Resources.PerQuarter;
                    break;
                case 6:
                    MonthPeriodAsString = Properties.Resources.PerHalfYear;
                    break;
                case 12:
                    MonthPeriodAsString = Properties.Resources.PerHalfYear;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        public string ValueAsString
        {
            get { return _valueAsString; }
            set { SetBackingField("ValueAsString", ref _valueAsString, value); }
        }

        public string MonthPeriodAsString
        {
            get { return _monthPeriodAsString; }
            set { SetBackingField("MonthPeriodAsString", ref _monthPeriodAsString, value); }
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }
    }
}
