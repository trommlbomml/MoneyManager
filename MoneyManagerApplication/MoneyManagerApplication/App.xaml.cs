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

            var applicationViewModel = new ApplicationViewModel(repository);
            applicationViewModel.ActivateRequestmanagementScreen();

            var mainWindow = new MainWindow {DataContext = applicationViewModel};

            MainWindow = mainWindow;
            MainWindow.Show();
        }
    }
}
