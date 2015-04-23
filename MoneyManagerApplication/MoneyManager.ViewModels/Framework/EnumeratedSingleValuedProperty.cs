using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
            var indexOfToDelete = _selectableValues.IndexOf(value);
            var indexOfSelected = _selectableValues.IndexOf(Value);
            
            _selectableValues.RemoveAt(indexOfToDelete);
            if (indexOfToDelete == indexOfSelected)
            {
                if (_selectableValues.Count == 0)
                {
                    Value = default(TPropertyType);
                }
                else if (indexOfSelected == _selectableValues.Count)
                {
                    Value = _selectableValues.Last();
                }
                else
                {
                    Value = _selectableValues[indexOfSelected];
                }
            }
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
