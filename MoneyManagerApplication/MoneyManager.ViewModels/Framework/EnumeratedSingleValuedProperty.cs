using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MoneyManager.ViewModels.Framework
{
    public class EnumeratedSingleValuedProperty<TPropertyType> : SingleValuedProperty<TPropertyType>
    {
        private readonly ObservableCollection<TPropertyType> _selectableValues;

        public EnumeratedSingleValuedProperty()
        {
            _selectableValues = new ObservableCollection<TPropertyType>();
            SelectableValues = new ReadOnlyObservableCollection<TPropertyType>(_selectableValues);
        }
        
        public ReadOnlyObservableCollection<TPropertyType> SelectableValues { get; private set; }

        public void AddValue(TPropertyType value)
        {
            _selectableValues.Add(value);
        }

        public void RemoveSelectedValue()
        {
            _selectableValues.Remove(Value);
            Value = default(TPropertyType);
        }

        public void RemoveValue(TPropertyType value)
        {
            _selectableValues.Remove(value);
        }

        public void SetRange(IEnumerable<TPropertyType> values)
        {
            _selectableValues.Clear();
            Value = default(TPropertyType);
            foreach (var property in values)
            {
                _selectableValues.Add(property);
            }
        }
    }
}
