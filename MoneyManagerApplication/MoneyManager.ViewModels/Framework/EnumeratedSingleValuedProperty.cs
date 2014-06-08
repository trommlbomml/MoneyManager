using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MoneyManager.ViewModels.Framework
{
    public class EnumeratedSingleValuedProperty<TPropertyType> : ViewModel
    {
        private readonly ObservableCollection<TPropertyType> _selectableValues;
        private TPropertyType _selectedValue;

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

        public void SetRange(IEnumerable<TPropertyType> values)
        {
            _selectableValues.Clear();
            SelectedValue = default(TPropertyType);
            foreach (var property in values)
            {
                _selectableValues.Add(property);
            }
        }

        public TPropertyType SelectedValue
        {
            get { return _selectedValue; }
            set { SetBackingField("SelectedValue", ref _selectedValue, value); }
        }
    }
}
