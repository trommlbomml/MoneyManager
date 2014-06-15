using System;
using System.Windows;
using MoneyManager.Model;
using MoneyManager.ViewModels;
using MoneyManagerApplication.ApplicationSettings;

namespace MoneyManagerApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var repository = RepositoryFactory.CreateRepository();

            var applicationSettingsImp = new ApplicationSettingsImp();

            var applicationViewModel = new ApplicationViewModel(repository, applicationSettingsImp, new WindowManagerImp());
            applicationViewModel.ActivateAccountManagementPage();

            var mainWindow = new MainWindow {DataContext = applicationViewModel};

            MainWindow = mainWindow;
            MainWindow.Show();
        }
    }
}
