using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MoneyManager.Interfaces;

namespace MoneyManager.Model
{
    class StandingOrderEntityImp : Entity, StandingOrderEntity
    {
        public StandingOrderEntityImp()
        {
            Description = string.Empty;
            LastBookDate = DateTime.MaxValue;
        }

        public StandingOrderEntityImp(XElement standingOrderElement, IEnumerable<CategoryEntity> categories):
            base(standingOrderElement.Attribute("Id").Value)
        {
            FirstBookDate = DateTime.Parse(standingOrderElement.Attribute("FirstBookDate").Value, CultureInfo.InvariantCulture);
            ReferenceMonth = int.Parse(standingOrderElement.Attribute("ReferenceMonth").Value);
            ReferenceDay = int.Parse(standingOrderElement.Attribute("ReferenceDay").Value);
            MonthPeriodStep = int.Parse(standingOrderElement.Attribute("MonthPeriodStep").Value);
            Description = standingOrderElement.Attribute("Description").Value;
            LastBookDate = DateTime.Parse(standingOrderElement.Attribute("LastBookDate").Value, CultureInfo.InvariantCulture);
            LastBookedDate = DateTime.Parse(standingOrderElement.Attribute("LastBookedDate").Value, CultureInfo.InvariantCulture);
            Value = Double.Parse(standingOrderElement.Attribute("Value").Value, CultureInfo.InvariantCulture);
            var categoryEntitiyId = standingOrderElement.Attribute("CategoryId").Value;
            if (!string.IsNullOrEmpty(categoryEntitiyId))
            {
                Category = categories.Single(c => c.PersistentId == categoryEntitiyId);
            }
        }

        public DateTime FirstBookDate { get;  set; }
        public DateTime LastBookedDate { get; set; }
        public DateTime LastBookDate { get; set; }
        public int ReferenceMonth { get;  set; }
        public int ReferenceDay { get;  set; }
        public int MonthPeriodStep { get;  set; }
        public double Value { get;  set; }
        public string Description { get;  set; }
        public CategoryEntity Category { get;  set; }

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

        public XElement Serialize()
        {
            return new XElement("StandingOrder",
                new XAttribute("Id", PersistentId),
                new XAttribute("FirstBookDate", FirstBookDate.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("LastBookedDate", LastBookedDate.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("LastBookDate", LastBookDate.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("ReferenceMonth", ReferenceMonth),
                new XAttribute("ReferenceDay", ReferenceDay),
                new XAttribute("MonthPeriodStep", MonthPeriodStep),
                new XAttribute("Description", Description ?? ""),
                new XAttribute("Value", Value.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("CategoryId", Category != null ? Category.PersistentId : ""));
        }

        public bool AnyMonthUpToIsInPeriod(int month)
        {
            var periodMonths = GetPeriodMonths();
            return Enumerable.Range(1, month).Any(periodMonths.Contains);
        }
    }
}
