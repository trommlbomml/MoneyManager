using System;
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

        private void ExecuteWithErrorHandling(Action action, Action onError = null)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (onError == null)
                {
                    Application.WindowManager.ShowError("Error", ex.Message);
                }
                else
                {
                    onError();
                }
            }
        }

        private void OnOpenAccountCommand()
        {
            ExecuteWithErrorHandling(() =>
            {
                var result = Application.WindowManager.ShowOpenFileDialog(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                if (string.IsNullOrEmpty(result)) return;
                Application.Repository.Open(result);
                Application.ApplicationSettings.UpdateRecentAccountInformation(result, DateTime.Now);
                Application.ActivateRequestmanagementPage();
            });
        }

        private void OnOpenRecentAccountCommand()
        {
            ExecuteWithErrorHandling(() =>
            {
                Application.Repository.Open(Accounts.SelectedValue.Path);
                Application.ApplicationSettings.UpdateRecentAccountInformation(Accounts.SelectedValue.Path, DateTime.Now);
                Application.ActivateRequestmanagementPage(); 
            }, HandleOpenRecentFailed);
        }

        private void HandleOpenRecentFailed()
        {
            Application.WindowManager.ShowQuestion(Properties.Resources.RemoveRecentAccountMessageCaption,
                string.Format(Properties.Resources.RemoveRecentAccountMessageFormat, Accounts.SelectedValue.Path),
                () =>
                {
                    Application.ApplicationSettings.DeleteRecentAccountInformation(Accounts.SelectedValue.Path);
                    Accounts.RemoveValueSelectedValue();
                }, () => {});
        }

        private void OnCreateNewAccountCommand()
        {
            var newAccountDialog = new CreateAccountDialogViewModel(Application, o => { }, OnCreateAccountDialogOk);
            Application.WindowManager.ShowDialog(newAccountDialog);
        }

        private void OnCreateAccountDialogOk(CreateAccountDialogViewModel dlg)
        {
            ExecuteWithErrorHandling(() =>
            {
                Application.Repository.Create(dlg.Path, dlg.Name);
                Application.ApplicationSettings.UpdateRecentAccountInformation(dlg.Path, DateTime.Now);
                Application.ActivateRequestmanagementPage();
            });
        }

        public CommandViewModel CreateNewAccountCommand { get; private set; }

        public CommandViewModel OpenRecentAccountCommand { get; private set; }

        public CommandViewModel OpenAccountCommand { get; private set; }
    }
}
