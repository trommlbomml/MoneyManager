using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MoneyManager.Interfaces;

namespace MoneyManager.Model.Entities
{
    internal class RequestEntityImp : Entity, RequestEntity
    {
        private double _value;
        private string _description;
        private DateTime _date;
        private CategoryEntity _category;

        public RequestEntityImp()
        {
            Description = string.Empty;
        }

        public RequestEntityImp(XElement requestElement, IEnumerable<CategoryEntity> categories):
            base(requestElement.Attribute("Id").Value)
        {
            _description = requestElement.Attribute("Description").Value;
            _value = Double.Parse(requestElement.Attribute("Value").Value, CultureInfo.InvariantCulture);
            _date = DateTime.Parse(requestElement.Attribute("Date").Value, CultureInfo.InvariantCulture);

            var categoryEntitiyId = requestElement.Attribute("CategoryId").Value;
            if (!string.IsNullOrEmpty(categoryEntitiyId))
            {
                _category = categories.Single(c => c.PersistentId == categoryEntitiyId);
            }
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

        public DateTime Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        public CategoryEntity Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }

        public RequestEntityPersistenceImp Clone()
        {
            return new RequestEntityPersistenceImp(this);
        }
    }
}
