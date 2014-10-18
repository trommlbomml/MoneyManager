using System;

namespace MoneyManager.ViewModels.Framework
{
    public class SingleValuedProperty<TPropertyType> : ViewModel
    {
        private TPropertyType _value;
        private bool _isEnabled;

        public event Action OnValueChanged;

        public TPropertyType Value
        {
            get { return _value; }
            set { SetBackingField("Value", ref _value, value, o => InvokeOnValueChanged()); }
        }

        private void InvokeOnValueChanged()
        {
            if (OnValueChanged != null) OnValueChanged();
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetBackingField("IsEnabled", ref _isEnabled, value); }
        }
    }
}
