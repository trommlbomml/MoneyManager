using System;
using System.Globalization;
using System.Xml.Linq;
using MoneyManager.Interfaces;

namespace MoneyManager.Model.Entities
{
    internal class RequestEntityPersistenceImp : RequestEntity
    {
        public string PersistentId { get; private set; }
        public double Value { get; private set; }
        public string Description { get; private set; }
        public DateTime Date { get; private set; }
        public CategoryEntity Category { get; private set; }

        public RequestEntityPersistenceImp(RequestEntity entity)
        {
            PersistentId = entity.PersistentId;
            Value = entity.Value;
            Description = entity.Description;
            Date = entity.Date;
            Category = new CategoryEntityStub(entity.Category);
        }

        public XElement Serialize()
        {
            return new XElement("Request",
                new XAttribute("Id", PersistentId),
                new XAttribute("Date", Date.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("Description", Description ?? ""),
                new XAttribute("Value", Value.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("CategoryId", Category != null ? Category.PersistentId : ""));
        }
    }
}