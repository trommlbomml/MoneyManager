using System;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels
{
    public class ApplicationViewModel : ViewModel
    {
        private string _windowTitle;
        private ScreenModel _activeScreen;
        public Repository Repository { get; private set; }

        public ApplicationViewModel(Repository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");

            Repository = repository;

            WindowTitle = Properties.Resources.ApplicationMainWindowTitle;
        }

        public void ActivateRequestmanagementScreen()
        {
            var currentDateTime = DateTime.Now;

            var requestManagementScreen = new RequestManagementScreenModel(this, currentDateTime.Year, currentDateTime.Month);
            ActiveScreen = requestManagementScreen;
        }

        public string WindowTitle
        {
            get { return _windowTitle; }
            set { SetBackingField("WindowTitle", ref _windowTitle, value); }
        }

        public ScreenModel ActiveScreen
        {
            get { return _activeScreen; }
            private set { SetBackingField("ActiveScreen", ref _activeScreen, value); }
        }
    }
}
