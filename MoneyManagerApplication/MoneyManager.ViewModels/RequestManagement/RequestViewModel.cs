﻿using System;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.RequestManagement
{
    public class RequestViewModel : EntityViewModel
    {
        private DateTime _date;
        private string _description;
        private double _value;
        private string _valueAsString;
        private string _dateAsString;
        private string _category;
        private string _categoryPersistentId;
        private bool _isStandingOrder;

        public RequestViewModel(ApplicationViewModel application, string entityId) : base(application, entityId)
        {
        }

        public DateTime Date
        {
            get { return _date; }
            set { SetBackingField("Date", ref _date, value, o => OnDateChanged()); }
        }

        private void OnDateChanged()
        {
            DateAsString = string.Format(Properties.Resources.RequestDateFormat, Date);
        }

        public string Description
        {
            get { return _description; }
            set { SetBackingField("Description", ref _description, value); }
        }

        public double Value
        {
            get { return _value; }
            set
            {
                SetBackingField("Value", ref _value, value);
                OnValueChanged();
            }
        }

        private void OnValueChanged()
        {
            ValueAsString = string.Format(Properties.Resources.MoneyValueFormat, Value);
        }

        public string ValueAsString
        {
            get { return _valueAsString; }
            set { SetBackingField("ValueAsString", ref _valueAsString, value); }
        }

        public string DateAsString
        {
            get { return _dateAsString; }
            private set { SetBackingField("DateAsString", ref _dateAsString, value); }
        }

        public string Category
        {
            get { return _category; }
            set { SetBackingField("Category", ref _category, value); }
        }

        public bool IsStandingOrder
        {
            get { return _isStandingOrder; }
            set { SetBackingField("IsStandingOrder", ref _isStandingOrder, value); }
        }

        public override void Refresh()
        {
            var entity = Application.Repository.QueryRequest(EntityId);
            Date = entity.Date;
            
            var categorySource = entity.Category;
            
            IsStandingOrder = false;
            Value = entity.Value;
            Description = entity.Description;

            if (categorySource != null)
            {
                Category = categorySource.Name;
                _categoryPersistentId = categorySource.PersistentId;
            }
            else
            {
                Category = Properties.Resources.NoCategory;
                _categoryPersistentId = null;
            }
        }

        public override void Save()
        {
            Application.Repository.UpdateRequest(EntityId, new RequestEntityData
            {
                Date = Date,
                Description = Description,
                Value = Value,
                CategoryPersistentId = _categoryPersistentId
            });
        }
    }
}
