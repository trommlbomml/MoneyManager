using System.ComponentModel;
using MoneyManager.ViewModels;

namespace MoneyManagerApplication
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Closing += OnMainWindowClosing;
        }

        private void OnMainWindowClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            var application = (ApplicationViewModel) DataContext;
            cancelEventArgs.Cancel = application.OnClosingRequest();
        }
    }
}
