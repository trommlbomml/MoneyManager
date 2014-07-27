using System;
using System.Linq;
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
        private string _caption;

        public RequestDialogViewModel(ApplicationViewModel application, int year, int month, Action<RequestDialogViewModel> onOk)
        {
            if (month < 1 || month > 12) throw new ArgumentException(@"Month index must be in range 1 to 12", "month");

            InitializeViewModel(application, year, month, null, onOk);
        }

        public RequestDialogViewModel(ApplicationViewModel application, string persistentId, Action<RequestDialogViewModel> onOk)
        {
            var request = application.Repository.QueryRequest(persistentId);
            Description = request.Description;
            Value = request.Value;
            Date = request.Date;

            var selectedCategoryId = request.Category != null ? request.Category.PersistentId : null;

            InitializeViewModel(application, request.Date.Year, request.Date.Month, selectedCategoryId, onOk);
        }
        
        private void InitializeViewModel(ApplicationViewModel application, int year, int month, string selectedCategoryId, Action<RequestDialogViewModel> onOk)
        {
            Date = new DateTime(year, month, 1);
            FirstPossibleDate = new DateTime(year, month, 1);
            LastPossibleDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            Categories = new EnumeratedSingleValuedProperty<CategoryViewModel>();

            foreach (var category in application.Repository.QueryAllCategories().Select(c => new CategoryViewModel(application, c.PersistentId)).OrderBy(c => c.Name))
            {
                Categories.AddValue(category);
                category.Refresh();
            }

            if (!string.IsNullOrEmpty(selectedCategoryId))
            {
                Categories.SelectedValue = Categories.SelectableValues.Single(c => c.EntityId == selectedCategoryId);
            }

            CreateRequestCommand = new CommandViewModel(() => onOk(this));

            UpdateLocalizedProperties();
        }

        public CommandViewModel CreateRequestCommand { get; private set; }

        public EnumeratedSingleValuedProperty<CategoryViewModel> Categories { get; private set; }

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

        public string Caption
        {
            get { return _caption; }
            set { SetBackingField("Caption", ref _caption, value); }
        }
    }
}
