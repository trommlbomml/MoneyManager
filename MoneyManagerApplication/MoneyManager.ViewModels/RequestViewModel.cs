using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels
{
    public class RequestViewModel : EntityViewModel
    {
        private DateTime _date;
        private string _description;
        private double _value;

        public RequestViewModel(ApplicationViewModel application, string entityId) : base(application, entityId)
        {
        }

        public DateTime Date
        {
            get { return _date; }
            set { SetBackingField("Date", ref _date, value); }
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

        public override void Refresh()
        {
            throw new NotImplementedException();
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }
    }
}
