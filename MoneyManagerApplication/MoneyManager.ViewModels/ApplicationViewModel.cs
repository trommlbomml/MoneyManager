using System;
using System.IO;
using System.Net.Mime;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.AccountManagement;
using MoneyManager.ViewModels.Framework;
using MoneyManager.ViewModels.RequestManagement;

namespace MoneyManager.ViewModels
{
    public class ApplicationViewModel : ViewModel
    {
        private PageViewModel _activePage;

        public Repository Repository { get; private set; }
        public WindowManager WindowManager { get; private set; }
        public ApplicationContext ApplicationContext { get; private set; }

        public ApplicationViewModel(Repository repository, ApplicationContext applicationContext, WindowManager windowManager)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (applicationContext == null) throw new ArgumentNullException("applicationContext");
            if (windowManager == null) throw new ArgumentNullException("windowManager");

            Repository = repository;    
            WindowManager = windowManager;
            ApplicationContext = applicationContext;
        }

        public void StartWithAutoLogon(string moneyManagerFilePath)
        {
            var success = false;
            try
            {
                Repository.Open(moneyManagerFilePath);
                var createdRequests = Repository.UpdateStandingOrdersToCurrentMonth(ApplicationContext.Now.Year, ApplicationContext.Now.Month);
                ActivateRequestmanagementPage(createdRequests);
                success = true;
            }
            catch (FileNotFoundException)
            {
                var message = string.Format("Datei '{0}' konnte nicht gefunden werden. Wählen Sie Datei erneut aus.", moneyManagerFilePath);
                WindowManager.ShowError(Properties.Resources.ErrorOpenRecentAccount, message);
            }
            catch (FileLockedException)
            {
                var message = string.Format(Properties.Resources.RecentAccountLockedFormat, moneyManagerFilePath);
                WindowManager.ShowError(Properties.Resources.ErrorOpenRecentAccount, message);
            }
            catch (Exception ex)
            {
                var message = string.Format(Properties.Resources.RecentAccountUnexpectedErrorFormat, ex.Message);
                WindowManager.ShowError(Properties.Resources.ErrorOpenRecentAccount, message);
            }

            if (!success)
            {
                ActivateAccountManagementPage();
            }
        }

        public void ActivateRequestmanagementPage(string[] createdRequests)
        {
            var currentDateTime = ApplicationContext.Now;
            var requestManagementScreen = new RequestManagementPageViewModel(this, currentDateTime.Year, currentDateTime.Month);
            ActivePage = requestManagementScreen;

            if (createdRequests != null && createdRequests.Length > 0)
            {
                WindowManager.ShowDialog(new CreatedRequestsDialogViewModel(this, createdRequests));
            }
        }

        public void ActivateAccountManagementPage()
        {
            var accountManagementScreen = new AccountManagementPageViewModel(this);
            ActivePage = accountManagementScreen;
        }

        public PageViewModel ActivePage
        {
            get { return _activePage; }
            internal set { SetBackingField("ActivePage", ref _activePage, value); }
        }

        public bool OnClosingRequest()
        {
            return ActivePage != null && ActivePage.OnClosingRequest();
        }
    }
}
