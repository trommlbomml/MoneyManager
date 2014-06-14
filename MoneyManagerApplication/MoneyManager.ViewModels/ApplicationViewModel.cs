using System;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels
{
    public class ApplicationViewModel : ViewModel
    {
        private string _windowTitle;
        private PageViewModel _activePage;

        public Repository Repository { get; private set; }
        public ApplicationSettings ApplicationSettings { get; private set; }

        public ApplicationViewModel(Repository repository, ApplicationSettings applicationSettings)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (applicationSettings == null) throw new ArgumentNullException("applicationSettings");

            Repository = repository;
            ApplicationSettings = applicationSettings;

            WindowTitle = Properties.Resources.ApplicationMainWindowTitle;
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

        public string WindowTitle
        {
            get { return _windowTitle; }
            set { SetBackingField("WindowTitle", ref _windowTitle, value); }
        }

        public PageViewModel ActivePage
        {
            get { return _activePage; }
            private set { SetBackingField("ActivePage", ref _activePage, value); }
        }
    }
}
