using System;
using System.Windows;
using MoneyManager.Model;
using MoneyManager.ViewModels;

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
            applicationSettingsImp.UpdateRecentAccountInformation(@"C:\Test\lalala.test", DateTime.Now);
            applicationSettingsImp.UpdateRecentAccountInformation(@"C:\asdasdsad\lalala.test", DateTime.Now.AddDays(-10));

            var applicationViewModel = new ApplicationViewModel(repository, applicationSettingsImp);
            applicationViewModel.ActivateAccountManagementPage();

            var mainWindow = new MainWindow {DataContext = applicationViewModel};

            MainWindow = mainWindow;
            MainWindow.Show();
        }
    }
}
