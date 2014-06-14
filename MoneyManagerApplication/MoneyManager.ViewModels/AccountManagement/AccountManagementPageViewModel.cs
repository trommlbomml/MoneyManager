using System.ComponentModel;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.AccountManagement
{
    public class AccountManagementPageViewModel : PageViewModel
    {
        public EnumeratedSingleValuedProperty<RecentAccountViewModel> Accounts { get; private set; } 

        public AccountManagementPageViewModel(ApplicationViewModel application) : base(application)
        {
            Accounts = new EnumeratedSingleValuedProperty<RecentAccountViewModel>();
            CreateAccountsEntries();

            CreateNewAccountCommand = new CommandViewModel(OnCreateNewAccountCommand);
            OpenRecentAccountCommand = new CommandViewModel(OnOpenRecentAccountCommand);
            OpenAccountCommand = new CommandViewModel(OnOpenAccountCommand);

            UpdateCommandStates();

            Caption = Properties.Resources.AccountManagementPageCaption;

            Accounts.PropertyChanged += OnAccountsChanged;
        }

        private void CreateAccountsEntries()
        {
            foreach (var recentAccount in Application.ApplicationSettings.RecentAccounts)
            {
                Accounts.AddValue(new RecentAccountViewModel
                {
                    LastAccessDate = recentAccount.LastAccessDate,
                    Path = recentAccount.Path
                });
            }
        }

        private void OnAccountsChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateCommandStates();
        }

        private void UpdateCommandStates()
        {
            CreateNewAccountCommand.IsEnabled = true;
            OpenRecentAccountCommand.IsEnabled = Accounts.SelectedValue != null;
            OpenAccountCommand.IsEnabled = true;
        }

        private void OnOpenAccountCommand()
        {
            Application.ActivateRequestmanagementPage();
        }

        private void OnOpenRecentAccountCommand()
        {
            Application.ActivateRequestmanagementPage();
        }

        private void OnCreateNewAccountCommand()
        {
            var newAccountDialog = new CreateAccountDialogViewModel(o => { }, OnCreateAccountDialogOk);
            Application.WindowManager.ShowDialog(newAccountDialog);
        }

        private void OnCreateAccountDialogOk(CreateAccountDialogViewModel dlg)
        {
            Application.ActivateRequestmanagementPage();
        }

        public CommandViewModel CreateNewAccountCommand { get; private set; }

        public CommandViewModel OpenRecentAccountCommand { get; private set; }

        public CommandViewModel OpenAccountCommand { get; private set; }
    }
}
