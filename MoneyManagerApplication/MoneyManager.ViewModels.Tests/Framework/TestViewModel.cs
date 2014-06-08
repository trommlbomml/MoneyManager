using System;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.Tests.Framework
{
    class TestViewModel : ViewModel
    {
        public bool TestSetBackingField<T>(string propertyName, ref T backingField, T value, Action<T> onChanged = null)
        {
            return SetBackingField(propertyName, ref backingField, value, onChanged);
        }

        public void TestOnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
    }
}