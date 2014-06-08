using System;
using System.ComponentModel;

namespace MoneyManager.ViewModels.Framework
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetBackingField<T>(string propertyName, ref T backingField, T value, Action<T> onValueChanged = null)
        {
            if (Equals(backingField, value)) return false;

            var oldValue = backingField;
            backingField = value;
            OnPropertyChanged(propertyName);
            if (onValueChanged != null) onValueChanged(oldValue);

            return true;
        }
    }
}
