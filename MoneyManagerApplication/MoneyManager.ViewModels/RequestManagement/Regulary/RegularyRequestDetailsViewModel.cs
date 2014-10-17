using System;
using System.Linq;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.RequestManagement.Regulary
{
    public class RegularyRequestDetailsViewModel : ViewModel
    {
        private bool _isInEditMode;
        private string _description;
        private string _entityId;
        private readonly ApplicationViewModel _application;
        private double _value;
        private DateTime _firstBookDate;
        public EnumeratedSingleValuedProperty<MonthPeriod> MonthPeriods { get; private set; }
        public EnumeratedSingleValuedProperty<CategoryViewModel> Categories { get; private set; }

        public RegularyRequestDetailsViewModel(ApplicationViewModel application, Action<RegularyRequestEntityData> onSave, Action<RegularyRequestDetailsViewModel> onCancel)
        {
            _application = application;
            SaveCommand = new CommandViewModel(() => onSave(CreateRegularyRequestEntityData()));
            CancelCommand = new CommandViewModel(() => onCancel(this));

            MonthPeriods = new EnumeratedSingleValuedProperty<MonthPeriod>();
            foreach (MonthPeriod value in Enum.GetValues(typeof(MonthPeriod)))
            {
                MonthPeriods.AddValue(value);
            }

            Categories = new EnumeratedSingleValuedProperty<CategoryViewModel>();
            foreach (var category in application.Repository.QueryAllCategories().Select(c => new CategoryViewModel(application, c.PersistentId)))
            {
                Categories.AddValue(category);
                category.Refresh();
            }

            FirstBookDate = application.ApplicationContext.Now.Date;
            
            UpdateCommandStates();
        }

        private static int GetPeriodFromEnum(MonthPeriod period)
        {
            switch (period)
            {
                case MonthPeriod.Monthly:
                    return 1;
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

        private RegularyRequestEntityData CreateRegularyRequestEntityData()
        {
            return new RegularyRequestEntityData
            {
                Description = Description,
                Value = Value,
                MonthPeriodStep = GetPeriodFromEnum(MonthPeriods.SelectedValue),
                CategoryEntityId = Categories.SelectedValue != null ? Categories.SelectedValue.EntityId : null,
                FirstBookDate = FirstBookDate,
                ReferenceMonth = FirstBookDate.Month,
                ReferenceDay = FirstBookDate.Day
            };
        }

        public CommandViewModel SaveCommand { get; private set; }
        public CommandViewModel CancelCommand { get; private set; }

        public void Refresh()
        {
            var request = _application.Repository.QueryRegularyRequest(EntityId);
            Description = request.Description;
            Value = request.Value;
            MonthPeriods.SelectedValue = GetEnumFromPeriod(request.MonthPeriodStep);
            Categories.SelectedValue = request.Category != null ? Categories.SelectableValues.Single(c => c.EntityId == request.Category.PersistentId) : null;
        }

        private static MonthPeriod GetEnumFromPeriod(int monthPeriodStep)
        {
            switch (monthPeriodStep)
            {
                case 1:
                    return MonthPeriod.Monthly;
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
        }

        public string EntityId
        {
            get { return _entityId; }
             set { SetBackingField("EntityId", ref _entityId, value); }
        }

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

        public DateTime FirstBookDate
        {
            get { return _firstBookDate; }
            set { SetBackingField("FirstBookDate", ref _firstBookDate, value); }
        }

        public bool IsInEditMode
        {
            get { return _isInEditMode; }
            set { SetBackingField("IsInEditMode",ref _isInEditMode, value, o => UpdateCommandStates()); }
        }
    }
}
