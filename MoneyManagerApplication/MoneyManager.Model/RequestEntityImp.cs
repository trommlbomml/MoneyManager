using System;
using System.Globalization;
using System.Xml.Linq;
using MoneyManager.Interfaces;

namespace MoneyManager.Model
{
    internal class RequestEntityImp : Entity, RequestEntity
    {
        public RequestEntityImp()
        {
            Description = string.Empty;
            Date = DateTime.Now.Date;
        }

        public RequestEntityImp(XElement requestElement):
            base(requestElement.Attribute("Id").Value)
        {
            Description = requestElement.Attribute("Description").Value;
            Value = Double.Parse(requestElement.Attribute("Value").Value, CultureInfo.InvariantCulture);
            Date = DateTime.Parse(requestElement.Attribute("Date").Value, CultureInfo.InvariantCulture);
        }

        public double Value { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public XElement Serialize()
        {
            return new XElement("Request", 
                new XAttribute("Id", PersistentId),
                new XAttribute("Date", Date.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("Description", Description ?? ""),
                new XAttribute("Value", Value.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
