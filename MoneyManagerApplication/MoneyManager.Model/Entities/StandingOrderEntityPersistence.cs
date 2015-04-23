using System;
using System.Globalization;
using System.Xml.Linq;
using MoneyManager.Interfaces;

namespace MoneyManager.Model.Entities
{
    class StandingOrderEntityPersistence : StandingOrderEntity
    {
        public string PersistentId { get; private set; }
        public DateTime FirstBookDate { get; private set; }
        public DateTime LastBookedDate { get; private set; }
        public DateTime LastBookDate { get; private set; }
        public int ReferenceMonth { get; private set; }
        public int ReferenceDay { get; private set; }
        public int MonthPeriodStep { get; private set; }
        public double Value { get; private set; }
        public string Description { get; private set; }
        public CategoryEntity Category { get; private set; }

        public StandingOrderEntityPersistence(StandingOrderEntity entity)
        {
            PersistentId = entity.PersistentId;
            FirstBookDate = entity.FirstBookDate;
            LastBookDate = entity.LastBookDate;
            LastBookedDate = entity.LastBookedDate;
            ReferenceMonth = entity.ReferenceMonth;
            ReferenceDay = entity.ReferenceDay;
            MonthPeriodStep = entity.MonthPeriodStep;
            Value = entity.Value;
            Description = entity.Description;
            Category = new CategoryEntityStub(entity.Category);
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
    }
}