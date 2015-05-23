using System;
using System.ComponentModel;

namespace MoneyManager.ViewModels.Framework
{
    public class SingleValuedProperty<TPropertyType> : ViewModel, IDataErrorInfo
    {
        private TPropertyType _value;
        private bool _isEnabled;

        public event Action OnValueChanged;
        public event Action OnIsValidChanged;
        public Func<string> Validate;
        private bool _isValid;
        private string _errorMessage;

        public SingleValuedProperty()
        {
            IsEnabled = true;
        }

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

        public bool IsValid
        {
            get { return _isValid; }
            set { SetBackingField("IsValid", ref _isValid, value, o => InvokeOnIsValidChanged()); }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            private set { SetBackingField("ErrorMessage", ref _errorMessage, value); }
        }

        private void InvokeOnIsValidChanged()
        {
            if (OnIsValidChanged != null) OnIsValidChanged();
        }

        public string this[string columnName]
        {
            get
            {
                var errorMessage = string.Empty;
                if (columnName == "Value")
                {
                    if (Validate != null) errorMessage = Validate();
                }

                IsValid = string.IsNullOrEmpty(errorMessage);
                ErrorMessage = errorMessage;
                return errorMessage;
            }
        }

        public string Error { get; private set; }
    }
}
