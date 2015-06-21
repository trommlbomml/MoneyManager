using System;
using System.Collections.ObjectModel;
using System.IO;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.AccountManagement
{
    public class AccountManagementPageViewModel : PageViewModel
    {
        private readonly ObservableCollection<RecentAccountViewModel> _accounts;
        private string _newAccountFilePath;

        public AccountManagementPageViewModel(ApplicationViewModel application) : base(application)
        {
            _accounts = new ObservableCollection<RecentAccountViewModel>();
            Accounts = new ReadOnlyObservableCollection<RecentAccountViewModel>(_accounts);
            CreateAccountsEntries();

            NewAccountNameProperty = new SingleValuedProperty<string>();
            CreateNewAccountCommand = new CommandViewModel(OnCreateNewAccountCommand);
            OpenAccountCommand = new CommandViewModel(OnOpenAccountCommand);
            SelectFileCommand = new CommandViewModel(OnSelectFileCommand);

            UpdateCommandStates();

            Caption = Properties.Resources.AccountManagementPageCaption;
        }
        
        public SingleValuedProperty<string> NewAccountNameProperty { get; private set; }
        public ReadOnlyObservableCollection<RecentAccountViewModel> Accounts { get; private set; }
        public CommandViewModel CreateNewAccountCommand { get; private set; }
        public CommandViewModel OpenAccountCommand { get; private set; }
        public CommandViewModel SelectFileCommand { get; private set; }

        public string NewAccountFilePath
        {
            get { return _newAccountFilePath; }
            internal set { SetBackingField("NewAccountFilePath", ref _newAccountFilePath, value, o => UpdateCommandStates()); }
        }

        private void OnSelectFileCommand()
        {
            var result = Application.WindowManager.ShowSaveFileDialog(System.IO.Path.GetDirectoryName(NewAccountFilePath), NewAccountFilePath, Properties.Resources.AccountManagementFilterOpenAccount);
            if (!string.IsNullOrEmpty(result)) NewAccountFilePath = result;
        }

        private void UpdateCommandStates()
        {
            CreateNewAccountCommand.IsEnabled = !string.IsNullOrWhiteSpace(NewAccountNameProperty.Value) && !string.IsNullOrWhiteSpace(NewAccountFilePath);
        }
        
        private void CreateAccountsEntries()
        {
            foreach (var recentAccount in Application.ApplicationContext.RecentAccounts)
            {
                _accounts.Add(new RecentAccountViewModel(OnOpenRecentAccountCommand, OnRemoveRecentAccountCommand)
                {
                    LastAccessDate = recentAccount.LastAccessDate,
                    Path = recentAccount.Path
                });
            }
        }
        
        private void OnRemoveRecentAccountCommand(RecentAccountViewModel account)
        {
            Application.WindowManager.ShowQuestion(Properties.Resources.AccountManagementRemoveAccountCaption,
                                                   Properties.Resources.AccountManagementRemoveAccountMessage,
            () =>
            {
                Application.ApplicationContext.DeleteRecentAccountInformation(account.Path);
                _accounts.Remove(account);
            }, () => { });
        }

        private void ExecuteWithErrorHandling(Action action, Action<Exception> onError = null)
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
                    onError(ex);
                }
            }
        }

        private void OnOpenAccountCommand()
        {
            ExecuteWithErrorHandling(() =>
            {
                var result = Application.WindowManager.ShowOpenFileDialog(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Properties.Resources.AccountManagementFilterOpenAccount);
                if (string.IsNullOrEmpty(result)) return;
                Application.Repository.Open(result);
                Application.Repository.UpdateStandingOrdersToCurrentMonth();
                Application.ActivateRequestmanagementPage();
            });
        }

        private void OnOpenRecentAccountCommand(RecentAccountViewModel account)
        {
            try
            {
                Application.Repository.Open(account.Path);
                Application.Repository.UpdateStandingOrdersToCurrentMonth();
                Application.ActivateRequestmanagementPage();
            }
            catch (FileLockedException)
            {
                var message = string.Format(Properties.Resources.RecentAccountLockedFormat, account.Path);
                Application.WindowManager.ShowError(Properties.Resources.ErrorOpenRecentAccount, message);
            }
            catch (FileNotFoundException)
            {
                var message = string.Format(Properties.Resources.RecentAccountNotFoundFormat, account.Path);
                Application.WindowManager.ShowQuestion(Properties.Resources.ErrorOpenRecentAccount,
                                                       message,
                                                        () =>
                                                        {
                                                            Application.ApplicationContext.DeleteRecentAccountInformation(account.Path);
                                                            _accounts.Remove(account);
                                                        }, () => { });
            }
            catch (Exception ex)
            {
                var message = string.Format(Properties.Resources.RecentAccountUnexpectedErrorFormat, ex.Message);
                Application.WindowManager.ShowError(Properties.Resources.ErrorOpenRecentAccount, message);
            }
        }
        
        private void OnCreateNewAccountCommand()
        {
            ExecuteWithErrorHandling(() =>
            {
                Application.Repository.Create(NewAccountFilePath, NewAccountNameProperty.Value);
                NewAccountNameProperty.Value = string.Empty;
                NewAccountFilePath = string.Empty;
                Application.ActivateRequestmanagementPage();
            });
        }
    }
}
