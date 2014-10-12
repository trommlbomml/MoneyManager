using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MoneyManager.Interfaces;

namespace MoneyManager.Model
{
    class RegularyRequestEntityImp : Entity, RegularyRequestEntity
    {
        public RegularyRequestEntityImp()
        {
            Description = string.Empty;
        }

        public RegularyRequestEntityImp(XElement regularyRequestElement, IEnumerable<CategoryEntity> categories):
            base(regularyRequestElement.Attribute("Id").Value)
        {
            FirstBookDate = DateTime.Parse(regularyRequestElement.Attribute("FirstBookDate").Value, CultureInfo.InvariantCulture);
            ReferenceMonth = int.Parse(regularyRequestElement.Attribute("ReferenceMonth").Value);
            ReferenceDay = int.Parse(regularyRequestElement.Attribute("ReferenceDay").Value);
            MonthPeriodStep = int.Parse(regularyRequestElement.Attribute("MonthPeriodStep").Value);
            Description = regularyRequestElement.Attribute("Description").Value;
            Value = Double.Parse(regularyRequestElement.Attribute("Value").Value, CultureInfo.InvariantCulture);
            var categoryEntitiyId = regularyRequestElement.Attribute("CategoryId").Value;
            if (!string.IsNullOrEmpty(categoryEntitiyId))
            {
                Category = categories.Single(c => c.PersistentId == categoryEntitiyId);
            }
        }

        public DateTime FirstBookDate { get;  set; }
        
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

        public RequestEntity CreateRequest(int year)
        {
            return new RequestEntityImp
            {
                Category = Category,
                Date = new DateTime(year, ReferenceMonth, ReferenceDay),
                Description = Description,
                Value = Value,
                RegularyRequest = this
            };
        }

        public XElement Serialize()
        {
            return new XElement("RegularyRequest",
                new XAttribute("Id", PersistentId),
                new XAttribute("FirstBookDate", FirstBookDate.ToString(CultureInfo.InvariantCulture)),
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
