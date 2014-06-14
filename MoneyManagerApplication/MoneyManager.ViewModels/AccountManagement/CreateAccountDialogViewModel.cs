using System;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.AccountManagement
{
    public class CreateAccountDialogViewModel : ViewModel
    {
        private string _path;
        private string _name;

        public CreateAccountDialogViewModel(Action<CreateAccountDialogViewModel> cancel,
                                            Action<CreateAccountDialogViewModel> ok)
        {
            CreateAccountCommand = new CommandViewModel(() => ok(this));
            CancelCommand = new CommandViewModel(() => cancel(this));

            UpdateCommandStates();
        }

        private void UpdateCommandStates()
        {
            CancelCommand.IsEnabled = true;
            CreateAccountCommand.IsEnabled = !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Path);
        }

        public CommandViewModel CreateAccountCommand { get; private set; }
        public CommandViewModel CancelCommand { get; private set; }

        public string Path
        {
            get { return _path; }
            set { SetBackingField("Path", ref _path, value, o => UpdateCommandStates()); }
        }

        public string Name
        {
            get { return _name; }
            set { SetBackingField("Name", ref _name, value, o => UpdateCommandStates()); }
        }
    }
}
