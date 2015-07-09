using System.Windows;
using MoneyManager.Model;
using MoneyManager.ViewModels;
using MoneyManagerApplication.ApplicationSettings;

namespace MoneyManagerApplication
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var applicationContextImp = new ApplicationContextImp();

            var repository = RepositoryFactory.CreateRepository(applicationContextImp);
            var applicationViewModel = new ApplicationViewModel(repository, applicationContextImp, new WindowManagerImp());
            var mainWindow = new MainWindow { DataContext = applicationViewModel };
            MainWindow = mainWindow;

            if (e.Args.Length == 1)
            {
                applicationViewModel.StartWithAutoLogon(e.Args[0]);
            }
            else
            {
                applicationViewModel.ActivateAccountManagementPage();
            }
            
            MainWindow.Show();
        }
    }
}
