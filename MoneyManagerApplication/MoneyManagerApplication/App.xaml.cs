using System.Windows;

namespace MoneyManagerApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = new MainWindow();
            MainWindow = mainWindow;

            MainWindow.Show();
        }
    }
}
