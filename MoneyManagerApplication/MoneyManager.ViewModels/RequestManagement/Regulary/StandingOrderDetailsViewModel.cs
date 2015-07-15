using System;
using System.Linq;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.RequestManagement.Regulary
{
    public class StandingOrderDetailsViewModel : ViewModel
    {
        private bool _isInEditMode;
        private string _entityId;
        private readonly ApplicationViewModel _application;
        private string _lastBookDateAsString;
        private double _calculateValue;
        private string _caption;

        public EnumeratedSingleValuedProperty<MonthPeriod> MonthPeriods { get; private set; }
        public EnumeratedSingleValuedProperty<CategoryViewModel> Categories { get; private set; }
        public EnumeratedSingleValuedProperty<RequestKind> RequestKind { get; private set; } 
        public SingleValuedProperty<double> ValueProperty { get; private set; }
        public SingleValuedProperty<int> PaymentsProperty { get; private set; }
        public SingleValuedProperty<bool> IsEndingTransactionProperty { get; private set; }
        public SingleValuedProperty<DateTime> FirstBookDateProperty { get; private set; } 
        public SingleValuedProperty<string> DescriptionProperty { get; private set; }

        public CommandViewModel SaveCommand { get; private set; }
        public CommandViewModel CancelCommand { get; private set; }

        public StandingOrderDetailsViewModel(ApplicationViewModel application, Action<StandingOrderEntityData> onSave, Action<StandingOrderDetailsViewModel> onCancel)
        {
            _application = application;
            SaveCommand = new CommandViewModel(() => onSave(CreateStandingOrderEntityData()));
            CancelCommand = new CommandViewModel(() => onCancel(this));
            PaymentsProperty = new SingleValuedProperty<int> { Value = 1 };
            ValueProperty = new SingleValuedProperty<double>();
            IsEndingTransactionProperty = new SingleValuedProperty<bool>();
            MonthPeriods = new EnumeratedSingleValuedProperty<MonthPeriod>();
            Categories = new EnumeratedSingleValuedProperty<CategoryViewModel>();
            FirstBookDateProperty = new SingleValuedProperty<DateTime>();
            DescriptionProperty = new SingleValuedProperty<string>();
            RequestKind = new EnumeratedSingleValuedProperty<RequestKind>();

            IsEndingTransactionProperty.OnValueChanged += OnIsEndingTransactionPropertyChanged;
            MonthPeriods.OnValueChanged += OnMonthPeriodsPropertyChanged;
            PaymentsProperty.OnValueChanged += OnPaymentsPropertyChanged;
            RequestKind.OnValueChanged += RequestKindOnOnValueChanged;
            ValueProperty.OnIsValidChanged += ValuePropertyOnOnIsValidChanged;
            ValueProperty.OnValueChanged += ValuePropertyOnOnValueChanged;
            ValueProperty.Validate = ValidateValueProperty;
            
            foreach (MonthPeriod value in Enum.GetValues(typeof(MonthPeriod)))
            {
                MonthPeriods.AddValue(value);
            }

            var allCategoryViewModels = application.Repository.QueryAllCategories().Select(c => new CategoryViewModel(application, c.PersistentId)).ToList();
            allCategoryViewModels.ForEach(a => a.Refresh());
            Categories.SetRange(allCategoryViewModels.OrderBy(c => c.Name));
            FirstBookDateProperty.Value = application.ApplicationContext.Now.Date;

            RequestKind.SetRange(Enum.GetValues(typeof(RequestKind)).Cast<RequestKind>());
            RequestKind.Value = RequestManagement.RequestKind.Expenditure;
            
            UpdateCommandStates();
            UpdateCaption();
        }

        public string Caption
        {
            get { return _caption; }
            private set { SetBackingField("Caption", ref _caption, value); }
        }

        private string ValidateValueProperty()
        {
            return ValueProperty.Value <= 0.0 ? Properties.Resources.RequestDialogViewModel_ValuePropertyValidationError : null;
        }

        private void ValuePropertyOnOnIsValidChanged()
        {
            UpdateCalculatedProperties();
            UpdateCommandStates();
        }

        private void ValuePropertyOnOnValueChanged()
        {
            UpdateCalculatedProperties();
        }

        private void RequestKindOnOnValueChanged()
        {
            UpdateCalculatedProperties();
        }

        private void OnIsEndingTransactionPropertyChanged()
        {
            UpdateCalculatedProperties();
            UpdateCommandStates();
        }

        private void OnMonthPeriodsPropertyChanged()
        {
            UpdateCalculatedProperties();
        }

        private void OnPaymentsPropertyChanged()
        {
            UpdateCalculatedProperties();
        }

        private static int GetPeriodFromEnum(MonthPeriod period)
        {
            switch (period)
            {
                case MonthPeriod.Monthly:
                    return 1;
                case MonthPeriod.TwoMonthly:
                    return 2;
                case MonthPeriod.Quarterly:
                    return 3;
                case MonthPeriod.HalfYearly:
                    return 6;
                case MonthPeriod.Yearly:
                    return 12;
                default:
                    throw new InvalidOperationException();
            }
        }

        private StandingOrderEntityData CreateStandingOrderEntityData()
        {
            return new StandingOrderEntityData
            {
                Description = DescriptionProperty.Value,
                Value = ValueProperty.Value * (RequestKind.Value == RequestManagement.RequestKind.Earning ? 1.0 : -1.0),
                MonthPeriodStep = GetPeriodFromEnum(MonthPeriods.Value),
                CategoryEntityId = Categories.Value != null ? Categories.Value.EntityId : null,
                FirstBookDate = FirstBookDateProperty.Value,
                ReferenceMonth = FirstBookDateProperty.Value.Month,
                ReferenceDay = FirstBookDateProperty.Value.Day,
                PaymentCount = IsEndingTransactionProperty.Value ? PaymentsProperty.Value : (int?) null
            };
        }

        public void Refresh()
        {
            var request = _application.Repository.QueryStandingOrder(EntityId);
            DescriptionProperty.Value = request.Description;
            ValueProperty.Value = Math.Abs(request.Value);
            MonthPeriods.Value = GetEnumFromPeriod(request.MonthPeriodStep);
            Categories.Value = request.Category != null ? Categories.SelectableValues.Single(c => c.EntityId == request.Category.PersistentId) : null;
            RequestKind.Value = request.Value > 0 ? RequestManagement.RequestKind.Earning : RequestManagement.RequestKind.Expenditure;
            IsEndingTransactionProperty.Value = MonthDifference(DateTime.MaxValue, request.LastBookDate) > 0;

            if (IsEndingTransactionProperty.Value)
            {
                PaymentsProperty.Value = MonthDifference(request.LastBookDate, request.FirstBookDate);
            }
        }

        private static int MonthDifference(DateTime lValue, DateTime rValue)
        {
            return (lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year);
        }


        public double CalculateValue
        {
            get { return _calculateValue; }
            private set { SetBackingField("CalculateValue", ref _calculateValue, value); }
        }

        private static MonthPeriod GetEnumFromPeriod(int monthPeriodStep)
        {
            switch (monthPeriodStep)
            {
                case 1:
                    return MonthPeriod.Monthly;
                case 2:
                    return MonthPeriod.TwoMonthly;
                case 3:
                    return MonthPeriod.Quarterly;
                case 6:
                    return MonthPeriod.HalfYearly;
                case 12:
                    return MonthPeriod.Yearly;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void UpdateCommandStates()
        {
            SaveCommand.IsEnabled = IsInEditMode;
            CancelCommand.IsEnabled = IsInEditMode;
            IsEndingTransactionProperty.IsEnabled = IsInEditMode;
            PaymentsProperty.IsEnabled = IsInEditMode && IsEndingTransactionProperty.Value;
        }

        public string EntityId
        {
             get { return _entityId; }
             set { SetBackingField("EntityId", ref _entityId, value, s => UpdateCaption()); }
        }

        private void UpdateCaption()
        {
            Caption = string.IsNullOrEmpty(EntityId) ? Properties.Resources.StandingOrderDetailsViewModel_NewStandingOrderCaption 
                                                     : Properties.Resources.StandingOrderDetailsViewModel_ExistingStandingOrderCaption;
        }

        private void UpdateCalculatedProperties()
        {
            LastBookDateAsString = IsEndingTransactionProperty.Value
                ? string.Format(Properties.Resources.RequestDateFormat, FirstBookDateProperty.Value.AddMonths(GetPeriodFromEnum(MonthPeriods.Value)*PaymentsProperty.Value))
                : Properties.Resources.StandingOrderDialog_NoLastBookDate;

            CalculateValue = ValueProperty.Value *
                             (RequestKind.Value == RequestManagement.RequestKind.Expenditure ? -1.0 : 1.0);
        }

        public string LastBookDateAsString
        {
            get { return _lastBookDateAsString; }
            private set { SetBackingField("LastBookDateAsString", ref _lastBookDateAsString, value); }
        }

        public bool IsInEditMode
        {
            get { return _isInEditMode; }
            set { SetBackingField("IsInEditMode",ref _isInEditMode, value, o => UpdateCommandStates()); }
        }
    }
}
