﻿using System;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels
{
    public class RequestViewModel : EntityViewModel
    {
        private DateTime _date;
        private string _description;
        private double _value;
        private string _valueAsString;
        private string _dateAsString;

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
            set { SetBackingField("Value", ref _value, value, o => OnValueChanged()); }
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

        public override void Refresh()
        {
            var entity = Application.Repository.QueryRequest(EntityId);
            Date = entity.Date;
            Description = entity.Description;
            Value = entity.Value;
        }

        public override void Save()
        {
            Application.Repository.UpdateRequest(EntityId, new RequestEntityData
            {
                Date = Date,
                Description = Description,
                Value = Value
            });
        }
    }
}
