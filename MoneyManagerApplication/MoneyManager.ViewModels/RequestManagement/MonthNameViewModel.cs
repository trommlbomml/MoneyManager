using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.RequestManagement
{
    public class MonthNameViewModel : ViewModel
    {
        private string _name;
        private int _index;
        private bool _isEnabled;

        public MonthNameViewModel(string name, int index)
        {
            Name = name;
            Index = index;
        }

        public int Index
        {
            get { return _index; }
            private set { SetBackingField("Index", ref _index, value); }
        }

        public string Name
        {
            get { return _name; }
            private set { SetBackingField("Name", ref _name, value); }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetBackingField("IsEnabled", ref _isEnabled, value); }
        }
    }
}
