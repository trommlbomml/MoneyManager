﻿using System.Windows;
using MoneyManager.Model;
using MoneyManager.ViewModels;
using MoneyManagerApplication.ApplicationSettings;

namespace MoneyManagerApplication
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var fileLock = new SingleUserLockFileImp();
            var repository = RepositoryFactory.CreateRepository(fileLock);

            var applicationSettingsImp = new ApplicationSettingsImp();

            var applicationViewModel = new ApplicationViewModel(repository, applicationSettingsImp, new WindowManagerImp());
            applicationViewModel.ActivateAccountManagementPage();

            var mainWindow = new MainWindow {DataContext = applicationViewModel};

            MainWindow = mainWindow;
            MainWindow.Show();
        }
    }
}
