using System;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.AccountManagement
{
    public class CreateAccountDialogViewModel : ViewModel
    {
        private string _path;
        private string _name;
        private readonly ApplicationViewModel _applicationViewModel;

        public CreateAccountDialogViewModel(ApplicationViewModel application, 
                                            Action<CreateAccountDialogViewModel> cancel,
                                            Action<CreateAccountDialogViewModel> ok)
        {
            _applicationViewModel = application;

            CreateAccountCommand = new CommandViewModel(() => ok(this));
            CancelCommand = new CommandViewModel(() => cancel(this));
            SelectFileCommand = new CommandViewModel(OnSelectFileCommand);

            Name = Properties.Resources.AccountManagementMyAccountDefaultName;
            Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Konto.mmdb");

            UpdateCommandStates();
        }

        private void OnSelectFileCommand()
        {
            var result = _applicationViewModel.WindowManager.ShowSaveFileDialog(System.IO.Path.GetDirectoryName(Path), Path, Properties.Resources.AccountManagementFilterOpenAccount);
            if (!string.IsNullOrEmpty(result))
            {
                Path = result;
            }
        }

        private void UpdateCommandStates()
        {
            CancelCommand.IsEnabled = true;
            CreateAccountCommand.IsEnabled = !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Path);
        }

        public CommandViewModel CreateAccountCommand { get; private set; }
        public CommandViewModel CancelCommand { get; private set; }
        public CommandViewModel SelectFileCommand { get; private set; }

        public string Path
        {
            get { return _path; }
            internal set { SetBackingField("Path", ref _path, value, o => UpdateCommandStates()); }
        }

        public string Name
        {
            get { return _name; }
            set { SetBackingField("Name", ref _name, value, o => UpdateCommandStates()); }
        }
    }
}
