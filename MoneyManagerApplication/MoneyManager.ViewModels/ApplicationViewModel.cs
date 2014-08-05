using System;
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
        public ApplicationSettings ApplicationSettings { get; private set; }

        public ApplicationViewModel(Repository repository, ApplicationSettings applicationSettings, WindowManager windowManager)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (applicationSettings == null) throw new ArgumentNullException("applicationSettings");

            Repository = repository;
            WindowManager = windowManager;
            ApplicationSettings = applicationSettings;
        }

        public void ActivateRequestmanagementPage()
        {
            var currentDateTime = DateTime.Now;

            var requestManagementScreen = new RequestManagementPageViewModel(this, currentDateTime.Year, currentDateTime.Month);
            ActivePage = requestManagementScreen;
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
