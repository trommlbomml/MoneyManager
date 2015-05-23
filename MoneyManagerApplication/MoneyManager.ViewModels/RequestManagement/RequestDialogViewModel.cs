using System;
using System.Linq;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.RequestManagement
{
    public enum RequestKind
    {
        Earning,
        Expenditure
    }

    public class RequestDialogViewModel : ViewModel
    {
        private DateTime _firstPossibleDate;
        private DateTime _lastPossibleDate;
        private string _dateAsString;
        private string _caption;
        private double _calculateValue;

        public RequestDialogViewModel(ApplicationViewModel application, int year, int month, Action<RequestDialogViewModel> onOk)
        {
            if (month < 1 || month > 12) throw new ArgumentException(@"Month index must be in range 1 to 12", "month");

            InitializeViewModel(application, year, month, null, onOk);

            DateProperty.Value = new DateTime(year, month, 1);
        }

        public RequestDialogViewModel(ApplicationViewModel application, string persistentId, Action<RequestDialogViewModel> onOk)
        {
            var request = application.Repository.QueryRequest(persistentId);
            var selectedCategoryId = request.Category != null ? request.Category.PersistentId : null;
            InitializeViewModel(application, request.Date.Year, request.Date.Month, selectedCategoryId, onOk);

            DescriptionProperty.Value = request.Description;
            ValueProperty.Value = Math.Abs(request.Value);
            DateProperty.Value = request.Date;
            RequestKind.Value = request.Value > 0 ? RequestManagement.RequestKind.Earning : RequestManagement.RequestKind.Expenditure;
        }
        
        private void InitializeViewModel(ApplicationViewModel application, int year, int month, string selectedCategoryId, Action<RequestDialogViewModel> onOk)
        {
            Categories = new EnumeratedSingleValuedProperty<CategoryViewModel>();
            DescriptionProperty = new SingleValuedProperty<string>();
            ValueProperty = new SingleValuedProperty<double>();
            DateProperty = new SingleValuedProperty<DateTime>();
            CreateRequestCommand = new CommandViewModel(() => onOk(this));
            RequestKind = new EnumeratedSingleValuedProperty<RequestKind>();
            RequestKind.SetRange(Enum.GetValues(typeof(RequestKind)).Cast<RequestKind>());
            RequestKind.Value = RequestManagement.RequestKind.Expenditure; 

            DateProperty.OnValueChanged += DatePropertyOnOnValueChanged;
            RequestKind.OnValueChanged += RequestKindOnOnValueChanged;
            ValueProperty.Validate = ValidateValueProperty;
            ValueProperty.OnIsValidChanged += ValuePropertyOnOnIsValidChanged;

            FirstPossibleDate = new DateTime(year, month, 1);
            LastPossibleDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            
            foreach (var category in application.Repository.QueryAllCategories().Select(c => new CategoryViewModel(application, c.PersistentId)).OrderBy(c => c.Name))
            {
                Categories.AddValue(category);
                category.Refresh();
            }

            if (!string.IsNullOrEmpty(selectedCategoryId))
            {
                Categories.Value = Categories.SelectableValues.Single(c => c.EntityId == selectedCategoryId);
            }

            UpdateLocalizedProperties();
            UpdateCommandStates();
        }

        private void RequestKindOnOnValueChanged()
        {
            UpdateCalculatedProperties();
        }

        private void ValuePropertyOnOnIsValidChanged()
        {
            UpdateCalculatedProperties();
            UpdateCommandStates();
        }

        private void UpdateCommandStates()
        {
            CreateRequestCommand.IsEnabled = ValueProperty.IsValid;
        }

        private void DatePropertyOnOnValueChanged()
        {
            UpdateLocalizedProperties();
        }

        private void UpdateCalculatedProperties()
        {
            CalculateValue = ValueProperty.Value *
                             (RequestKind.Value == RequestManagement.RequestKind.Expenditure ? -1.0 : 1.0);
        }

        private string ValidateValueProperty()
        {
            return ValueProperty.Value <= 0.0 ? Properties.Resources.RequestDialogViewModel_ValuePropertyValidationError : null;
        }

        public CommandViewModel CreateRequestCommand { get; private set; }

        public EnumeratedSingleValuedProperty<CategoryViewModel> Categories { get; private set; }
        public EnumeratedSingleValuedProperty<RequestKind> RequestKind { get; private set; } 
        public SingleValuedProperty<string> DescriptionProperty { get; private set; }
        public SingleValuedProperty<double> ValueProperty { get; private set; }
        public SingleValuedProperty<DateTime> DateProperty { get; private set; } 

        private void UpdateLocalizedProperties()
        {
            DateAsString = string.Format(Properties.Resources.RequestDayOfMonthFormat, DateProperty.Value);
        }

        public double CalculateValue
        {
            get { return _calculateValue; }
            private set { SetBackingField("CalculateValue", ref _calculateValue, value); }
        }

        public string DateAsString
        {
            get { return _dateAsString; }
            private set { SetBackingField("DateAsString", ref _dateAsString, value); }
        }

        public DateTime FirstPossibleDate
        {
            get { return _firstPossibleDate; }
            private set { SetBackingField("FirstPossibleDate", ref _firstPossibleDate, value); }
        }

        public DateTime LastPossibleDate
        {
            get { return _lastPossibleDate; }
            private set { SetBackingField("LastPossibleDate", ref _lastPossibleDate, value); }
        }

        public string Caption
        {
            get { return _caption; }
            set { SetBackingField("Caption", ref _caption, value); }
        }
    }
}
