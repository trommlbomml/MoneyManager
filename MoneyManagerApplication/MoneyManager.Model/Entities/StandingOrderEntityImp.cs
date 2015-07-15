using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MoneyManager.Interfaces;

namespace MoneyManager.Model.Entities
{
    class StandingOrderEntityImp : Entity, StandingOrderEntity
    {
        private readonly ApplicationContext _applicationContext;
        private DateTime _firstBookDate;
        private DateTime _lastBookedDate;
        private DateTime _lastBookDate;
        private int _referenceMonth;
        private int _referenceDay;
        private int _monthPeriodStep;
        private double _value;
        private string _description;
        private CategoryEntity _category;

        public StandingOrderEntityImp(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;

            Description = string.Empty;
            LastBookDate = DateTime.MaxValue;
        }

        public StandingOrderEntityImp(ApplicationContext applicationContext, XElement standingOrderElement, IEnumerable<CategoryEntity> categories):
            base(standingOrderElement.Attribute("Id").Value)
        {
            _applicationContext = applicationContext;
            _firstBookDate = DateTime.Parse(standingOrderElement.Attribute("FirstBookDate").Value, CultureInfo.InvariantCulture);
            _referenceMonth = int.Parse(standingOrderElement.Attribute("ReferenceMonth").Value);
            _referenceDay = int.Parse(standingOrderElement.Attribute("ReferenceDay").Value);
            _monthPeriodStep = int.Parse(standingOrderElement.Attribute("MonthPeriodStep").Value);
            _description = standingOrderElement.Attribute("Description").Value;
            _lastBookDate = DateTime.Parse(standingOrderElement.Attribute("LastBookDate").Value, CultureInfo.InvariantCulture);
            _lastBookedDate = DateTime.Parse(standingOrderElement.Attribute("LastBookedDate").Value, CultureInfo.InvariantCulture);
            _value = Double.Parse(standingOrderElement.Attribute("Value").Value, CultureInfo.InvariantCulture);
            var categoryEntitiyId = standingOrderElement.Attribute("CategoryId").Value;
            if (!string.IsNullOrEmpty(categoryEntitiyId))
            {
                _category = categories.Single(c => c.PersistentId == categoryEntitiyId);
            }
        }

        public DateTime FirstBookDate
        {
            get { return _firstBookDate; }
            set { SetProperty(ref _firstBookDate, value); }
        }

        public DateTime LastBookedDate
        {
            get { return _lastBookedDate; }
            set { SetProperty(ref _lastBookedDate, value); }
        }

        public DateTime LastBookDate
        {
            get { return _lastBookDate; }
            set { SetProperty(ref _lastBookDate, value); }
        }

        public int ReferenceMonth
        {
            get { return _referenceMonth; }
            set { SetProperty(ref _referenceMonth,value); }
        }

        public int ReferenceDay
        {
            get { return _referenceDay; }
            set { SetProperty(ref _referenceDay, value); }
        }

        public int MonthPeriodStep
        {
            get { return _monthPeriodStep; }
            set { SetProperty(ref _monthPeriodStep, value); }
        }

        public double Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public StandingOrderState State
        {
            get
            {
                if (_applicationContext.Now.Date < FirstBookDate.Date) return StandingOrderState.InActive;
                if (_applicationContext.Now.Date >= LastBookDate.Date) return StandingOrderState.Finished;
                return StandingOrderState.Active;
            }
        }

        public CategoryEntity Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }

        public bool IsMonthOfPeriod(int month)
        {
            return Enumerable.Range(0, 12 / MonthPeriodStep)
                             .Select(i => (ReferenceMonth + i * MonthPeriodStep) % 13)
                             .Any(m => m == month);
        }

        public int[] GetPeriodMonths()
        {
            return Enumerable.Range(0, 12 / MonthPeriodStep)
                             .Select(i => (ReferenceMonth + i * MonthPeriodStep) % 13).ToArray();
        }

        public RequestEntityImp CreateRequest(DateTime bookDate)
        {
            return new RequestEntityImp
            {
                Category = Category,
                Date = bookDate,
                Description = Description,
                Value = Value
            };
        }

        public DateTime? GetNextPaymentDateTime()
        {
            if (LastBookedDate == DateTime.MinValue) return FirstBookDate;
            if (LastBookedDate == LastBookDate) return null;
            return LastBookedDate.AddMonths(MonthPeriodStep);
        }

        public bool AnyMonthUpToIsInPeriod(int month)
        {
            var periodMonths = GetPeriodMonths();
            return Enumerable.Range(1, month).Any(periodMonths.Contains);
        }

        public StandingOrderEntityPersistence Clone()
        {
            return new StandingOrderEntityPersistence(this);
        }
    }
}
