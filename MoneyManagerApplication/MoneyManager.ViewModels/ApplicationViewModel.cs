﻿using System;
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

        public void ActivateRequestmanagementPage()
        {
            var currentDateTime = ApplicationContext.Now;
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
