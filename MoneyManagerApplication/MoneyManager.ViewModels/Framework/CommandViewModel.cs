using System;
using System.Windows.Input;

namespace MoneyManager.ViewModels.Framework
{
    public class CommandViewModel : ViewModel, ICommand
    {
        private readonly Action _action;
        private bool _isEnabled;

        public CommandViewModel(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");

            _action = action;
            IsEnabled = true;
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetBackingField("IsEnabled", ref _isEnabled, value, o => OnCanExecuteChanged()); }
        }

        private void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public event EventHandler CanExecuteChanged;
    }
}
